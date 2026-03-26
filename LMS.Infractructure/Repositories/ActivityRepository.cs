using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;

public class ActivityRepository(ApplicationDbContext context) : RepositoryBase<Activity>(context), IActivityRepository
{
}