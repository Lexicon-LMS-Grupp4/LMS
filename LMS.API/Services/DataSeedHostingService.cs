using Bogus;
using Domain.Models.Enums;
using LMS.Infractructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace LMS.API.Services;

public class DataSeedHostingService : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IConfiguration configuration;
    private readonly ILogger<DataSeedHostingService> logger;
    private UserManager<ApplicationUser> userManager = null!;
    private RoleManager<IdentityRole> roleManager = null!;
    private ApplicationDbContext dbContext = null!;
    private const string TeacherRole = "Teacher";
    private const string StudentRole = "Student";
    private Random random = new Random();

    public DataSeedHostingService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<DataSeedHostingService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if (!env.IsDevelopment()) return;

        dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        bool hasUsers = await dbContext.Users.AnyAsync(cancellationToken);
        bool hasDocuments = await dbContext.Documents.AnyAsync(cancellationToken);

        if (hasUsers && hasDocuments) return;

        userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        ArgumentNullException.ThrowIfNull(roleManager, nameof(roleManager));
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));

        try
        {
            if (!hasUsers)
            {
                await AddRolesAsync([TeacherRole, StudentRole]);

                var courses = FakeCourses(5);
                await AddCoursesToDb(courses);

                var modules = FakeModules(2, 7, courses);
                await AddModulesToDb(modules);

                var activities = FakeActivities(modules);
                await AddActivitiesToDb(activities);

                await AddDemoUsersAsync(courses);
                await AddFakedUsersAsync(20, courses);
            }

            if (!hasDocuments)
            {
                var courses = await dbContext.Courses.ToListAsync(cancellationToken);
                var teachers = await dbContext.Users
                    .Where(u => dbContext.UserRoles
                        .Where(ur => dbContext.Roles
                            .Where(r => r.Name == TeacherRole)
                            .Select(r => r.Id)
                            .Contains(ur.RoleId))
                        .Select(ur => ur.UserId)
                        .Contains(u.Id))
                    .ToListAsync(cancellationToken);

                if (courses.Any() && teachers.Any())
                {
                    var documents = FakeDocuments(courses, teachers);
                    await AddDocumentsToDb(documents);
                }
            }

            logger.LogInformation("Seed complete");
        }
        catch (Exception ex)
        {
            logger.LogError($"Data seed fail with error: {ex.Message}");
            throw;
        }
    }

    private async Task AddRolesAsync(string[] rolenames)
    {
        foreach (string rolename in rolenames)
        {
            if (await roleManager.RoleExistsAsync(rolename)) continue;
            var role = new IdentityRole { Name = rolename };
            var res = await roleManager.CreateAsync(role);

            if (!res.Succeeded) throw new Exception(string.Join("\n", res.Errors));
        }
    }
    private async Task AddDemoUsersAsync(IEnumerable<Course> courses)
    {
        var faker = new Faker();

        var teachers = new List<ApplicationUser> {
            new(){ UserName = "teacher@test.com", Email = "teacher@test.com"},
            new(){ UserName = "teacher1@test.com", Email = "teacher1@test.com"},
            new(){ UserName = "teacher2@test.com", Email = "teacher2@test.com"},
            new(){ UserName = "teacher3@test.com", Email = "teacher3@test.com"},
            new(){ UserName = "teacher4@test.com", Email = "teacher4@test.com"},
            new(){ UserName = "teacher5@test.com", Email = "teacher5@test.com"}
        };

        await AddUserToDb(teachers, TeacherRole);

        foreach (var teacher in teachers)
        {
            var selectedCourses = faker.PickRandom(courses, faker.Random.Int(1, 3));
            teacher.TeachingCourses = selectedCourses
                .Select(c => new CourseTeacher
                {
                    CourseId = c.Id,
                    TeacherId = teacher.Id
                })
                .ToList();
        }
        await dbContext.SaveChangesAsync();

        var student = new ApplicationUser
        {
            UserName = "student@test.com",
            Email = "student@test.com",
            CourseId = courses.First().Id
        };

        await AddUserToDb([student], StudentRole);

    }

    private async Task AddFakedUsersAsync(int nrOfUsers, IEnumerable<Course> courses)
    {
        var faker = new Faker<ApplicationUser>("sv").Rules((f, e) =>
        {
            e.Email = f.Person.Email;
            e.UserName = f.Person.Email;
            e.CourseId = f.PickRandom(courses).Id;
        });

        await AddUserToDb(faker.Generate(nrOfUsers), StudentRole);
    }

    private async Task AddUserToDb(IEnumerable<ApplicationUser> users, string role)
    {
        var passWord = configuration["password"];
        ArgumentNullException.ThrowIfNull(passWord, nameof(passWord));

        foreach (var user in users)
        {
            var result = await userManager.CreateAsync(user, passWord);
            if (!result.Succeeded)
                throw new Exception(string.Join("\n", result.Errors));

            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                throw new Exception(string.Join("\n", roleResult.Errors));
        }

        await dbContext.SaveChangesAsync();
    }

    private IEnumerable<Course> FakeCourses(int nrOfCourses)
    {
        var subjects = new[]
            {
                "C#", ".NET", "Webbutveckling", "Databaser", "JavaScript",
                "React", "API-utveckling", "Molntjänster", "AI", "Testning"
            };

        var faker = new Faker<Course>("sv")
            .RuleFor(c => c.Name, f => $"{f.PickRandom(subjects)} {f.PickRandom(new[] { "Grund", "Fortsättning", "Avancerad" })}")
            .RuleFor(c => c.Description, f => f.Lorem.Paragraph(2))
            .RuleFor(c => c.StartDate, f => EnsureWeekday(f.Date.Soon(90)));

        return faker.Generate(nrOfCourses);
    }

    private async Task AddCoursesToDb(IEnumerable<Course> courses)
    {
        if (dbContext is null)
        {
            throw new InvalidOperationException("DbContext is not initialized");
        }
        foreach (var course in courses)
        {
            bool exists = await dbContext.Courses
                .AnyAsync(c => c.Name == course.Name);

            if (!exists)
            {
                await dbContext.Courses.AddAsync(course);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private IEnumerable<Domain.Models.Entities.Module> FakeModules(int nrOfModulesAtLEast, int nrOfModulesTops, IEnumerable<Course> courses)
    {
        var progression = new[]
            {
                "Introduktion",
                "Grundläggande koncept",
                "Fördjupning",
                "Praktisk tillämpning",
                "Avancerade tekniker",
                "Integration och helhet",
                "Projekt"
            };

        var modules = new List<Domain.Models.Entities.Module>();

        var textFaker = new Faker("sv");

        foreach (var course in courses)
        {
            int moduleCount = random.Next(nrOfModulesAtLEast, nrOfModulesTops);

            var moduleStart = course.StartDate;

            for (int i = 0; i < moduleCount; i++)
            {
                var moduleLength = random.Next(2, 21);
                var moduleEnd = EnsureWeekday(moduleStart.AddDays(moduleLength), 17);

                modules.Add(new Domain.Models.Entities.Module
                {
                    CourseId = course.Id,
                    Name = progression[i % 7],
                    Description = textFaker.Lorem.Paragraph(random.Next(1, 3)),
                    StartDate = moduleStart,
                    EndDate = moduleEnd
                });

                moduleStart = EnsureWeekday(moduleEnd.AddDays(1));
            }
        }

        return modules;
    }

    private async Task AddModulesToDb(IEnumerable<Module> modules)
    {
        if (dbContext is null)
        {
            throw new InvalidOperationException("DbContext is not initialized");
        }
        foreach (var module in modules)
        {
            bool exists = await dbContext.Modules
                .AnyAsync(c => c.Name == module.Name);

            if (!exists)
            {
                await dbContext.Modules.AddAsync(module);
            }
        }

        await dbContext.SaveChangesAsync();
    }
    private IEnumerable<Domain.Models.Entities.Activity> FakeActivities(IEnumerable<Module> modules)
    {
        var activities = new List<Domain.Models.Entities.Activity>();

        var textFaker = new Faker("sv");

        foreach (var module in modules)
        {
            var compareTime = module.StartDate;

            while (compareTime < module.EndDate)
            {
                var type = textFaker.PickRandom<ActivityType>();
                var (activityStart, activityEnd) = FakeActivityTimes(type, compareTime);
                compareTime = activityEnd;

                activities.Add(new Domain.Models.Entities.Activity
                {
                    ModuleId = module.Id,
                    Name = FakeActivityName(type, textFaker),
                    Type = type,
                    Description = textFaker.Lorem.Paragraph(random.Next(1, 3)),
                    StartTime = activityStart,
                    EndTime = activityEnd
                });
            }
        }

        return activities;
    }

    private (DateTime?, DateTime) FakeActivityTimes(ActivityType type, DateTime afterTime)
    {
        var startTime = EnsureWeekday(afterTime.AddDays(random.Next(1, 3)));
        return type switch
        {
            ActivityType.Lecture => (WithHour(9, startTime), WithHour(12, startTime)),
            ActivityType.Exercise => (WithHour(10, startTime), WithHour(15, startTime)),
            ActivityType.Assignment => (null, EnsureWeekday(afterTime.AddDays(random.Next(1, 5)))),
            ActivityType.ELearning => (null, EnsureWeekday(afterTime.AddDays(random.Next(1, 5)))),
            _ => (WithHour(9, startTime), WithHour(17, startTime))
        };
    }

    private string FakeActivityName(ActivityType type, Faker textFaker)
    {
        return type switch
        {
            ActivityType.Lecture => textFaker.PickRandom(new[]
            {
                "Introduktion till ämnet",
                "Genomgång av grunder",
                "Fördjupning i koncept",
                "Teori och principer"
            }),

            ActivityType.Exercise => textFaker.PickRandom(new[]
            {
                "Praktisk övning",
                "Kodövning",
                "Workshop",
                "Parprogrammering"
            }),

            ActivityType.Assignment => textFaker.PickRandom(new[]
            {
                "Inlämningsuppgift 1",
                "Projektuppgift",
                "Hemuppgift",
                "Case study"
            }),

            ActivityType.ELearning => textFaker.PickRandom(new[]
            {
                "Onlinekurs",
                "Videogenomgång",
                "Självstudier",
                "Interaktiv modul"
            }),

            _ => textFaker.PickRandom(new[]
            {
                "Seminarium",
                "Diskussion",
                "Övrig aktivitet"
            })
        };
    }

    private async Task AddActivitiesToDb(IEnumerable<Domain.Models.Entities.Activity> activities)
    {
        if (dbContext is null)
        {
            throw new InvalidOperationException("DbContext is not initialized");
        }
        foreach (var activity in activities)
        {
            bool exists = await dbContext.Modules
                .AnyAsync(c => c.Name == activity.Name);

            if (!exists)
            {
                await dbContext.Activities.AddAsync(activity);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private IEnumerable<Document> FakeDocuments(IEnumerable<Course> courses, IEnumerable<ApplicationUser> teachers)
    {
        var faker = new Faker("sv");
        var fileTypes = new[] { "pdf", "docx", "pptx", "xlsx", "txt" };
        var displayNames = new[]
        {
            "Lecture Notes", "Course Material", "Assignment Brief",
            "Study Guide", "Exam Prep", "Lab Instructions", "Reading List"
        };

        var documents = new List<Document>();
        var teacherList = teachers.ToList();

        foreach (var course in courses)
        {
            int docCount = random.Next(2, 5);
            var teacher = faker.PickRandom(teacherList);

            for (int i = 0; i < docCount; i++)
            {
                var name = faker.PickRandom(displayNames);
                var ext = faker.PickRandom(fileTypes);
                documents.Add(new Document
                {
                    CourseId = course.Id,
                    DisplayName = name,
                    FileName = $"{name.ToLower().Replace(' ', '-')}-{i + 1}.{ext}",
                    Description = faker.Lorem.Sentence(),
                    UploadedAt = faker.Date.Recent(30),
                    UploadedByUserId = teacher.Id
                });
            }
        }

        return documents;
    }

    private async Task AddDocumentsToDb(IEnumerable<Document> documents)
    {
        if (!await dbContext.Documents.AnyAsync())
        {
            await dbContext.Documents.AddRangeAsync(documents);
            await dbContext.SaveChangesAsync();
        }
    }

    private static DateTime EnsureWeekday(DateTime date, int atHour = 8)
    {
        var newDate = date.DayOfWeek switch
        {
            DayOfWeek.Saturday => date.AddDays(2),
            DayOfWeek.Sunday => date.AddDays(1),
            _ => date
        };
        return WithHour(atHour, newDate);
    }

    private static DateTime WithHour(int hour, DateTime date)
    {
        return new DateTime(
            date.Year,
            date.Month,
            date.Day,
            hour, // h
            0,  // m
            0   // s
        );
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
