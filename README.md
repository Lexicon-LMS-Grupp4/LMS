# .NET LMS student project

## Definition of Done (WIP)
- [ ] Code builds successfully without errors
- [ ] Relevant functionality is tested manually (happy path + basic edge cases)
- [ ] No obvious bugs or broken flows in the UI
- [ ] API endpoints return correct status codes and responses
- [ ] Code follows agreed structure and naming conventions
- [ ] Add description of the PR in the PR description field, including what was changed
- [ ] At least one team member has reviewed the PR before merge
- [ ] Changes are merged into the correct branch (e.g. `develop`/`master`)

## Database Schema
```mermaid
erDiagram
    APPLICATIONUSER {
        string Id "<+ IdentityUser fields>"
        int CourseId FK
    }

    COURSE {
        int Id PK
        string Name
        string Description
        datetime StartDate
    }

    MODULE {
        int Id PK
        int CourseId FK
        string Name
        string Description
        datetime StartDate
        datetime EndDate
    }

    ACTIVITY {
        int Id PK
        int ModuleId FK
        string Name
        string Description
        string Type
        datetime StartTime
        datetime EndTime
    }

    DOCUMENT {
        int Id PK
        string FileName
        string DisplayName
        string Description
        datetime UploadedAt
        string UploadedByUserId FK
        int CourseId FK
        int ModuleId FK
        int ActivityId FK
        int SubmissionId FK
    }

    SUBMISSION {
        int Id PK
        string StudentId FK
        int ActivityId FK
        string Body
        datetime SubmittedAt
        bool IsLate
        string FeedbackText
        datetime FeedbackGivenAt
    }

    COURSETEACHER {
        int CourseId PK, FK
        int TeacherId PK, FK
    }

    COURSE ||--o{ MODULE : has
    MODULE ||--o{ ACTIVITY : has
    COURSE ||--o{ APPLICATIONUSER : has_students
    APPLICATIONUSER ||--o{ DOCUMENT : uploads
    COURSE ||--o{ DOCUMENT : contains
    MODULE ||--o{ DOCUMENT : contains
    ACTIVITY ||--o{ DOCUMENT : contains
    APPLICATIONUSER ||--o{ SUBMISSION : makes
    DOCUMENT ||--o| SUBMISSION : attached_to
    APPLICATIONUSER ||--o{ SUBMISSION : reviews
    COURSE ||--o{ COURSETEACHER : has
    APPLICATIONUSER ||--o{ COURSETEACHER : teaches_in
```