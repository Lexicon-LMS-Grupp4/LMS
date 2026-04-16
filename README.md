# .NET LMS student project

## Definition of Done
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
    ApplicationUser ||--o{ Submission : submits
    ApplicationUser ||--o{ SubmissionComment : writes
    ApplicationUser ||--o{ Document : uploads
    ApplicationUser ||--o{ CourseTeacher : teaches

    Course ||--o{ Module : contains
    Course ||--o{ ApplicationUser : has_students
    Course ||--o{ CourseTeacher : has_teachers
    Course ||--o{ Document : has

    Module ||--o{ Activity : contains
    Module ||--o{ Document : has

    ActivityType ||--o{ Activity : classifies

    Activity ||--o{ Submission : receives
    Activity ||--o{ Document : has

    Submission ||--o{ SubmissionComment : has
    Submission ||--|| Document : may_have

    ApplicationUser {
        string Id PK
        int CourseId FK
    }

    Course {
        int Id PK
    }

    CourseTeacher {
        int CourseId PK, FK
        string TeacherId PK, FK
    }

    Module {
        int Id PK
        int CourseId FK
    }

    Activity {
        int Id PK
        int ModuleId FK
        int ActivityTypeId FK
    }

    ActivityType {
        int Id PK
    }

    Submission {
        int Id PK
        int ActivityId FK
        string StudentId FK
    }

    SubmissionComment {
        int Id PK
        int SubmissionId FK
        string AuthorId FK
    }

    Document {
        int Id PK
        int CourseId FK
        int ModuleId FK
        int ActivityId FK
        int SubmissionId FK
        string UploadedByUserId FK
    }
```