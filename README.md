<!-- REPLACE ALL THE [WolfOWI] TEXT WITH YOUR GITHUB PROFILE NAME & THE [coriander-backend] WITH THE NAME OF YOUR GITHUB PROJECT -->

<!-- Repository Information & Links-->
<br />

![GitHub repo size](https://img.shields.io/github/repo-size/WolfOWI/coriander-backend?color=88a764)
![GitHub watchers](https://img.shields.io/github/watchers/WolfOWI/coriander-backend?color=88a764)
![GitHub language count](https://img.shields.io/github/languages/count/WolfOWI/coriander-backend?color=88a764)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/WolfOWI/coriander-backend?color=88a764)
[![LinkedIn][linkedin-shield]][linkedin-url]
[![Instagram][instagram-shield]][instagram-url]
[![Behance][behance-shield]][behance-url]

<!-- HEADER SECTION -->
<h5 align="center" style="padding:0;margin:0;">Iné Smith - 221076</h5>
<h5 align="center" style="padding:0;margin:0;">Kayla Posthumus - 231096</h5>
<h5 align="center" style="padding:0;margin:0;">Ruan Klopper - 231280</h5>
<h5 align="center" style="padding:0;margin:0;">Wolf Botha - 21100255</h5>
<h6 align="center">Interactive Development 300 • 2025</h6>
</br>
<p align="center">

  <div align="center" href="https://github.com/WolfOWI/coriander-backend" >
    <img align="center" src="docs/readme-media/coricore-icon.png" alt="Coriander HR Backend Logo" height="140">
  </div>
  
  <h3 align="center">CoriCore API</h3>

  <p align="center">
    The RESTful API backend for the Coriander HR Management System built with ASP.NET Core and PostgreSQL. <br>
      <a href="https://github.com/WolfOWI/coriander-backend"><strong>Explore the docs »</strong></a>
   <br />
   <br />
   <a href="https://youtu.be/EvPt8mvmRxk">View Demo</a>
    ·
    <a href="https://github.com/WolfOWI/coriander-backend/issues">Report Bug</a>
    ·
    <a href="https://github.com/WolfOWI/coriander-backend/issues">Request Feature</a>
</p>

<!-- TABLE OF CONTENTS -->

## Table of Contents

- [Table of Contents](#table-of-contents)
- [About the Project](#about-the-project)
  - [Project Description](#project-description)
  - [Built With](#built-with)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Environment Variables](#environment-variables)
  - [Database Setup](#database-setup)
- [API Architecture](#api-architecture)
  - [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
  - [Authentication \& User Management](#authentication--user-management)
  - [Employee Management](#employee-management)
  - [Leave Management](#leave-management)
  - [Equipment Management](#equipment-management)
  - [Meeting \& Performance Management](#meeting--performance-management)
  - [Admin Management](#admin-management)
  - [Utility Endpoints](#utility-endpoints)
- [Database Schema](#database-schema)
  - [Core Entities](#core-entities)
  - [Relationships](#relationships)
- [Services \& Business Logic](#services--business-logic)
  - [Authentication Service](#authentication-service)
  - [User Service](#user-service)
  - [Employee Service](#employee-service)
  - [Admin Service](#admin-service)
  - [Leave Management Service](#leave-management-service)
  - [Equipment Management Service](#equipment-management-service)
  - [Meeting Service](#meeting-service)
  - [Performance Review Service](#performance-review-service)
  - [Gathering Service](#gathering-service)
  - [Page Service](#page-service)
  - [Email Service](#email-service)
  - [Image Service](#image-service)
  - [Integration Services](#integration-services)
- [Development Process](#development-process)
  - [Implementation Process](#implementation-process)
  - [Highlights](#highlights)
  - [Challenges](#challenges)
  - [Future Implementation](#future-implementation)
- [Deployment](#deployment)
  - [Docker Configuration](#docker-configuration)
  - [Production Deployment](#production-deployment)
- [API Documentation](#api-documentation)
  - [Swagger Integration](#swagger-integration)
  - [Video Demonstration](#video-demonstration)
- [Contributing](#contributing)
- [Authors](#authors)
- [License](#license)
- [Contact](#contact)
- [Acknowledgements](#acknowledgements)

<!--PROJECT DESCRIPTION-->

## About the Project

### Project Description

CoriCore API serves as the comprehensive server-side infrastructure for the Coriander HR Management System. Built with ASP.NET Core 9.0 and PostgreSQL, this RESTful API provides efficient endpoints for all HR operations including authentication, employee management, leave requests, equipment tracking, meeting scheduling, and performance reviews. The API is fully documented with Swagger/OpenAPI specifications and includes extensive unit and integration testing.

### Built With

**Core Framework & Database**

- **[ASP.NET Core 9.0](https://docs.microsoft.com/en-us/aspnet/core/)** - High-performance web API framework
- **[C#](https://docs.microsoft.com/en-us/dotnet/csharp/)** - Primary programming language
- **[PostgreSQL](https://www.postgresql.org/)** - Advanced open-source relational database
- **[Entity Framework Core 9.0](https://docs.microsoft.com/en-us/ef/core/)** - Object-relational mapping framework
- **[Npgsql](https://www.npgsql.org/)** - PostgreSQL data provider for .NET

**Authentication**

- **[JWT Bearer Authentication](https://jwt.io/)** - Stateless token-based authentication
- **[Google OAuth 2.0](https://developers.google.com/identity/protocols/oauth2)** - Google authentication integration
- **[BCrypt.Net](https://github.com/BcryptNet/bcrypt.net)** - Password hashing and verification

**External Integrations**

- **[MailKit](https://github.com/jstedfast/MailKit)** - Email sending and SMTP support
- **[RestSharp](https://restsharp.dev/)** - REST API client for external service integration

**API Documentation & Development Tools**

- **[Swagger/OpenAPI](https://swagger.io/)** - API documentation and testing interface
- **[Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)** - Swagger integration for ASP.NET Core
- **[DotNetEnv](https://github.com/tonerdo/dotnet-env)** - Environment variable management

**Testing Framework**

- **[xUnit](https://xunit.net/)** - Unit testing framework
- **[Moq](https://github.com/moq/moq4)** - Mocking framework for unit tests
- **[Coverlet](https://github.com/coverlet-coverage/coverlet)** - Code coverage analysis

**DevOps & Deployment**

- **[Docker](https://www.docker.com/)** - Containerisation platform
- **[Render](https://render.com/)** - Cloud deployment platform

<!-- GETTING STARTED -->

## Getting Started

The following instructions will get you a copy of the backend API up and running on your local machine for development and testing purposes.

### Prerequisites

Ensure you have the following installed on your machine:

- **[.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)** - Required for building and running the application
- **[PostgreSQL 13+](https://www.postgresql.org/download/)** - Database server (or access to a PostgreSQL instance)
- **[Git](https://git-scm.com/)** - Version control system
- **[Docker](https://www.docker.com/get-started)** (Optional) - For containerized deployment

### Installation

1. **Clone the Repository**

   ```sh
   git clone https://github.com/WolfOWI/coriander-backend.git
   cd coriander-backend
   ```

2. **Restore Dependencies**

   ```sh
   cd CoriCore
   dotnet restore
   ```

3. **Environment Configuration**

   ```sh
   # Copy the template file
   cp appsettings.Development.json.template appsettings.Development.json

   # Create a .env file for environment variables
   touch .env
   ```

4. **Database Migration**

   ```sh
   # Update database with latest migrations
   dotnet ef database update
   ```

5. **Run the Application**

   ```sh
   # Development mode (opens Swagger UI automatically)
   dotnet run

   # Or in watch mode for development
   dotnet watch run
   ```

6. **Verify Installation**
   - The API should be accessible at `http://localhost:5121/api/`

### Environment Variables

Create a `.env` file in the `CoriCore` directory with the following variables:

```env
# Database Configuration
PGHOST=your-postgresql-host
PGPORT=5432
PGUSER=your-username
PGPASSWORD=your-password
PGDATABASE=coriander_hr

# JWT Configuration
JWT_SECRET=your-super-secret-jwt-key-at-least-32-characters-long

# Google OAuth Configuration
GOOGLE_CLIENT_ID=your-google-oauth-client-id
GOOGLE_CLIENT_SECRET=your-google-oauth-client-secret

# Email Configuration
SMTP_HOST=your-smtp-server
SMTP_PORT=587
SMTP_USERNAME=your-email-username
SMTP_PASSWORD=your-email-password
FROM_EMAIL=noreply@yourcompany.com
FROM_NAME=Coriander HR System

# Cloudinary Configuration (for image uploads)
CLOUDINARY_CLOUD_NAME=your-cloudinary-cloud-name
CLOUDINARY_API_KEY=your-cloudinary-api-key
CLOUDINARY_API_SECRET=your-cloudinary-api-secret
```

### Database Setup

1. **Create PostgreSQL Database**

   ```sql
   CREATE DATABASE coriander_hr;
   CREATE USER coriander_user WITH PASSWORD 'your-password';
   GRANT ALL PRIVILEGES ON DATABASE coriander_hr TO coriander_user;
   ```

2. **Run Database Migrations**

   ```sh
   # Ensure you're in the CoriCore directory
   cd CoriCore

   # Apply migrations
   dotnet ef database update
   ```

3. **Seed Initial Data** (Optional)
   ```sh
   # Run the application once to create initial leave types and categories
   dotnet run
   ```

## API Architecture

### Project Structure

```
CoriCore/
├── Controllers/           # API Controllers (REST endpoints)
├── Services/             # Business logic services
├── Interfaces/           # Service interfaces and contracts
├── Models/              # Entity models and enums
├── DTOs/                # Data Transfer Objects
├── Data/                # Database context and configurations
├── Migrations/          # Entity Framework migrations
├── Properties/          # Application properties and settings
├── wwwroot/            # Static files and uploads
├── Program.cs          # Application entry point and configuration
├── appsettings.json    # Application configuration
└── Dockerfile          # Container configuration

CoriCore.Tests/
├── Controllers/        # Controller unit tests
├── Services/          # Service unit tests
├── Database/          # Database integration tests
└── CoverageReport/    # Test coverage reports
```

**Google OAuth Integration**

- Seamless login and registration with Google accounts
- Automatic profile picture and information retrieval
- Role-based account creation (Employee/Admin)

**Role-Based Access Control**

- Employee role: Limited to personal data and requests
- Admin role: Full system access and management capabilities
- Unassigned role: Pending approval accounts

## API Endpoints

### Authentication & User Management

**Authentication**

- **POST** `/api/Auth/register` - Register new unassigned user with email
- **POST** `/api/Auth/register-admin` - Register admin with email
- **POST** `/api/Auth/google-register-admin` - Register admin with Google
- **POST** `/api/Auth/register-admin-verified` - Complete admin registration after verification
- **POST** `/api/Auth/google-register` - Register with Google OAuth
- **POST** `/api/Auth/register-verified` - Complete registration after verification
- **POST** `/api/Auth/request-verification` - Request email verification code
- **POST** `/api/Auth/verify-email-code` - Verify email with code
- **POST** `/api/Auth/google-login` - Login with Google OAuth
- **POST** `/api/Auth/login` - Login with email/password
- **GET** `/api/Auth/me` - Get current user info
- **GET** `/api/Auth/decode-token` - Decode JWT token
- **POST** `/api/Auth/logout` - Logout user

**User Management**

- **GET** `/api/User` - Get all users
- **GET** `/api/User/{id}` - Get user by ID
- **PUT** `/api/User/{id}` - Update user information
- **DELETE** `/api/User/{id}` - Delete user
- **GET** `/api/User/unlinked` - Get users not linked to employees/admins

### Employee Management

**Core Employee Operations**

- **GET** `/api/Employee` - Get all employees
- **GET** `/api/Employee/{id}` - Get employee by ID
- **POST** `/api/Employee/setup-user-as-employee` - Setup existing user as employee
- **POST** `/api/Employee/suspension-toggle/{employeeId}` - Toggle employee suspension status
- **DELETE** `/api/Employee/{id}` - Delete employee
- **GET** `/api/Employee/status-totals` - Get employee status statistics

**Employee User Management**

- **GET** `/api/EmpUser` - Get all employee users with information
- **GET** `/api/EmpUser/{id}` - Get employee user by ID
- **PUT** `/api/EmpUser/edit-by-id/{id}` - Edit employee details
- **GET** `/api/EmpUser/equip-stats/{comparedEquipId}` - Get employee equipment statistics

### Leave Management

**Leave Requests**

- **GET** `/api/LeaveRequest` - Get all leave requests
- **GET** `/api/LeaveRequest/{id}` - Get specific leave request
- **GET** `/api/LeaveRequest/EmployeeId/{employeeId}` - Get employee's leave requests
- **POST** `/api/LeaveRequest/SubmitLeaveRequest` - Submit new leave request
- **DELETE** `/api/LeaveRequest/{id}` - Cancel leave request

**Leave Request Administration**

- **GET** `/api/EmpLeaveRequest/GetAllEmployeeLeaveRequests` - Get all employee leave requests
- **GET** `/api/EmpLeaveRequest/GetAllPending` - Get pending leave requests
- **GET** `/api/EmpLeaveRequest/GetAllApproved` - Get approved leave requests
- **GET** `/api/EmpLeaveRequest/GetAllRejected` - Get rejected leave requests
- **PUT** `/api/EmpLeaveRequest/ApproveLeaveRequestById/{leaveRequestId}` - Approve leave request
- **PUT** `/api/EmpLeaveRequest/RejectLeaveRequestById/{leaveRequestId}` - Reject leave request
- **PUT** `/api/EmpLeaveRequest/SetLeaveRequestToPendingById/{leaveRequestId}` - Set request to pending

**Leave Types & Balances**

- **GET** `/api/LeaveType` - Get all leave types
- **GET** `/api/LeaveType/{id}` - Get leave type by ID
- **GET** `/api/LeaveBalance/employee/{employeeId}` - Get employee's leave balances

### Equipment Management

**Equipment Items**

- **GET** `/api/Equipment` - Get all equipment items
- **GET** `/api/Equipment/by-empId/{employeeId}` - Get equipment by employee
- **GET** `/api/Equipment/unassigned` - Get unassigned equipment
- **POST** `/api/Equipment/CreateEquipmentItems` - Create multiple equipment items
- **PUT** `/api/Equipment/{id}` - Update equipment item
- **DELETE** `/api/Equipment/{id}` - Delete equipment item
- **POST** `/api/Equipment/assign-equipment/{employeeId}` - Assign equipment to employee
- **DELETE** `/api/Equipment/unlink/{equipmentId}` - Unlink equipment from employee
- **DELETE** `/api/Equipment/mass-unlink/{employeeId}` - Unlink all equipment from employee

**Equipment Categories**

- **GET** `/api/EquipmentCategory` - Get all equipment categories
- **POST** `/api/EquipmentCategory` - Create equipment category
- **GET** `/api/EquipmentCategory/{id}` - Get equipment category by ID
- **PUT** `/api/EquipmentCategory/{id}` - Update equipment category
- **DELETE** `/api/EquipmentCategory/{id}` - Delete equipment category

### Meeting & Performance Management

**Meetings**

- **GET** `/api/Meeting/GetAllRequestsByEmpId/{employeeId}` - Get employee's meeting requests
- **GET** `/api/Meeting/GetAllUpcomingByAdminId/{adminId}` - Get admin's upcoming meetings
- **GET** `/api/Meeting/GetAllPendingRequestsByAdminId/{adminId}` - Get admin's pending requests
- **POST** `/api/Meeting/CreateRequest` - Create meeting request
- **PUT** `/api/Meeting/ConfirmAndSchedule/{meetingId}` - Confirm and schedule meeting
- **PUT** `/api/Meeting/Update/{meetingId}` - Update meeting
- **PUT** `/api/Meeting/UpdateRequest/{meetingId}` - Update meeting request
- **PUT** `/api/Meeting/Reject/{meetingId}` - Reject meeting
- **PUT** `/api/Meeting/MarkAsCompleted/{meetingId}` - Mark meeting as completed
- **PUT** `/api/Meeting/MarkAsUpcoming/{meetingId}` - Mark meeting as upcoming
- **DELETE** `/api/Meeting/Delete/{meetingId}` - Delete meeting

**Performance Reviews**

- **GET** `/api/PerformanceReview` - Get all performance reviews
- **GET** `/api/PerformanceReview/{id}` - Get specific review
- **GET** `/api/PerformanceReview/GetPrmByStartDateAdminId/{adminId}/{startDate}` - Get reviews by date and admin
- **GET** `/api/PerformanceReview/GetPrmByEmpId/{employeeId}` - Get employee's reviews
- **GET** `/api/PerformanceReview/EmpUserRatingMetrics` - Get all employee rating metrics
- **GET** `/api/PerformanceReview/GetTopEmpUserRatingMetrics` - Get top rated employees metrics
- **GET** `/api/PerformanceReview/EmpUserRatingMetrics/{employeeId}` - Get specific employee metrics
- **GET** `/api/PerformanceReview/GetAllUpcomingPrm` - Get all upcoming reviews
- **GET** `/api/PerformanceReview/GetAllUpcomingPrmByAdminId/{adminId}` - Get admin's upcoming reviews
- **GET** `/api/PerformanceReview/top-rated-employees` - Get top rated employees
- **POST** `/api/PerformanceReview/CreatePerformanceReview` - Create new review
- **PUT** `/api/PerformanceReview/UpdatePerformanceReview/{id}` - Update review
- **PUT** `/api/PerformanceReview/update-status/{id}` - Update review status
- **DELETE** `/api/PerformanceReview/DeletePerformanceReview/{id}` - Delete review

**Gatherings (Meetings & Reviews Combined)**

- **GET** `/api/Gathering/all-by-empId/{employeeId}` - Get all employee gatherings
- **GET** `/api/Gathering/upcoming-by-empId/{employeeId}` - Get upcoming gatherings
- **GET** `/api/Gathering/completed-by-empId/{employeeId}` - Get completed gatherings
- **GET** `/api/Gathering/upcoming-and-completed-by-empId-desc/{employeeId}` - Get all gatherings sorted
- **GET** `/api/Gathering/upcoming-by-adminId/{adminId}` - Get admin's upcoming gatherings
- **GET** `/api/Gathering/completed-by-adminId/{adminId}` - Get admin's completed gatherings
- **GET** `/api/Gathering/by-adminId/{adminId}/month/{month}` - Get admin's gatherings by month

### Admin Management

- **POST** `/api/Admin/promote-existing-user-to-admin` - Promote user to admin
- **GET** `/api/Admin/admins` - Get all admins
- **GET** `/api/Admin/adminUser/{adminId}` - Get admin by ID

### Utility Endpoints

**Image Management**

- **POST** `/api/Image/upload` - Upload an image
- **POST** `/api/Image/profile-picture/{userId}` - Update user's profile picture
- **DELETE** `/api/Image/profile-picture/{userId}` - Remove user's profile picture

**Email**

- **POST** `/api/Email/send-test` - Send test email

**Health Check**

- **GET** `/api/Health` - Check API health status

**Page Information**

- **GET** `/api/Page/admin-emp-details/{employeeId}` - Get admin employee details page info
- **GET** `/api/Page/admin-emp-management` - Get admin employee management page info
- **GET** `/api/Page/employee-profile/{employeeId}` - Get employee profile page info
- **GET** `/api/Page/admin-dashboard/{adminId}` - Get admin dashboard info
- **GET** `/api/Page/employee-leave-overview/{employeeId}` - Get employee leave overview

## Database Schema

### Core Entities

**User Table**

```sql
- UserId (PK, Identity)
- FullName (varchar)
- Email (varchar, unique)
- Password (varchar, hashed)
- GoogleId (varchar, nullable)
- ProfilePicture (varchar, nullable)
- Role (enum: Unassigned, Employee, Admin)
- IsVerified (boolean)
- VerificationCode (varchar, nullable)
- CodeGeneratedAt (datetime, nullable)
```

**Admin Table**

```sql
- AdminId (PK, Identity)
- UserId (FK to Users)
```

**Employee Table**

```sql
- EmployeeId (PK, Identity)
- UserId (FK to Users)
- Gender (enum)
- DateOfBirth (date)
- PhoneNumber (varchar)
- JobTitle (varchar)
- Department (varchar)
- SalaryAmount (decimal)
- PayCycle (enum)
- LastPaidDate (date)
- EmployType (enum)
- EmployDate (date)
- IsSuspended (boolean)
```

**PerformanceReview Table**

```sql
- ReviewId (PK, Identity)
- AdminId (FK to Admins)
- EmployeeId (FK to Employees)
- IsOnline (boolean)
- MeetLocation (varchar)
- MeetLink (varchar)
- StartDate (datetime)
- EndDate (datetime)
- Rating (int)
- Comment (text)
- DocUrl (text)
- Status (enum: Upcoming, Completed)
```

**Meeting Table**

```sql
- MeetingId (PK, Identity)
- AdminId (FK to Admins)
- EmployeeId (FK to Employees)
- IsOnline (boolean)
- MeetLocation (varchar)
- MeetLink (varchar)
- StartDate (datetime)
- EndDate (datetime)
- Purpose (text)
- RequestedAt (datetime)
- Status (enum: Requested, Upcoming, Completed, Rejected)
```

**Equipment Table**

```sql
- EquipmentId (PK, Identity)
- EmployeeId (FK to Employees, nullable)
- EquipmentName (varchar)
- EquipmentCatId (FK to EquipmentCategories)
- AssignedDate (date, nullable)
- Condition (enum: New, Good, Fair, Poor, Damaged)
```

**EquipmentCategory Table**

```sql
- EquipmentCatId (PK, Identity)
- EquipmentCatName (varchar)
```

**LeaveRequest Table**

```sql
- LeaveRequestId (PK, Identity)
- EmployeeId (FK to Employees)
- LeaveTypeId (FK to LeaveTypes)
- StartDate (date)
- EndDate (date)
- Comment (text, nullable)
- Status (enum: Pending, Approved, Rejected)
```

**LeaveType Table**

```sql
- LeaveTypeId (PK, Identity)
- LeaveTypeName (varchar)
- Description (text)
- DefaultDays (int)
```

**LeaveBalance Table**

```sql
- LeaveBalanceId (PK, Identity)
- EmployeeId (FK to Employees)
- LeaveTypeId (FK to LeaveTypes)
- RemainingDays (decimal)
```

### Relationships

**User Relationships**

- User → Admin: One-to-One (User can be an Admin)
- User → Employee: One-to-One (User can be an Employee)

**Employee Relationships**

- Employee → LeaveRequests: One-to-Many (Employee can have multiple leave requests)
- Employee → Equipment: One-to-Many (Employee can have multiple equipment items)
- Employee → PerformanceReviews: One-to-Many (Employee can have multiple performance reviews)
- Employee → Meetings: One-to-Many (Employee can have multiple meetings)
- Employee → LeaveBalances: One-to-Many (Employee has balance for each leave type)

**Admin Relationships**

- Admin → PerformanceReviews: One-to-Many (Admin conducts multiple performance reviews)
- Admin → Meetings: One-to-Many (Admin conducts multiple meetings)

**Equipment Relationships**

- Equipment → EquipmentCategory: Many-to-One (Multiple equipment items can belong to one category)
- Equipment → Employee: Many-to-One (Multiple equipment items can be assigned to one employee)

**Leave Management Relationships**

- LeaveType → LeaveRequests: One-to-Many (One leave type can have multiple requests)
- LeaveType → LeaveBalances: One-to-Many (One leave type has balance for multiple employees)
- LeaveRequest → Employee: Many-to-One (Multiple requests can belong to one employee)
- LeaveBalance → Employee: Many-to-One (Multiple balances can belong to one employee)

## Services & Business Logic

### Authentication Service

**Core Authentication**

- Password hashing using BCrypt with salt
- JWT token generation and validation
- Token decoding and user information retrieval
- Session management and logout handling

**OAuth Integration**

- Google OAuth token verification
- Google profile information retrieval
- OAuth state management and validation

**Email Verification**

- Two-factor authentication via email
- Verification code generation and validation
- Code expiration management

### User Service

**User Management**

- User registration and profile creation
- Role management (Unassigned, Employee, Admin)
- Profile information updates
- User deletion and deactivation

**Profile Management**

- Profile picture upload and management
- User information retrieval
- Unlinked user management

### Employee Service

**Core Employee Operations**

- Employee registration and setup
- Employee profile management
- Department and position tracking
- Salary and payment cycle management

**Employment Status**

- Suspension management
- Employment type tracking
- Employee statistics and metrics
- Employee deletion with user role management

### Admin Service

**Admin Management**

- Admin registration and setup
- Admin profile management
- User promotion to admin
- Admin role verification

### Leave Management Service

**Leave Request Processing**

- Leave request submission and validation
- Leave duration calculation
- Leave status management (Pending, Approved, Rejected)
- Leave request cancellation

**Leave Balance Management**

- Leave balance tracking per type
- Balance updates on request approval
- Default balance creation for new employees
- Annual leave reset and carryover

**Leave Types**

- Leave type management
- Default days configuration
- Leave type description management

### Equipment Management Service

**Equipment Tracking**

- Equipment registration and cataloging
- Assignment tracking with timestamps
- Condition monitoring and updates
- Equipment transfer between employees
- Mass assignment and unassignment operations

**Equipment Categories**

- Category creation and management
- Equipment categorization
- Category-based reporting

### Meeting Service

**Meeting Management**

- Meeting request creation
- Meeting scheduling and confirmation
- Online/offline meeting setup
- Meeting status updates

**Meeting Logistics**

- Location management
- Online meeting link generation
- Meeting purpose tracking
- Meeting completion tracking

### Performance Review Service

**Review Management**

- Performance review scheduling
- Review documentation
- Rating system management
- Review status tracking

**Review Analytics**

- Employee rating metrics
- Top performers identification
- Performance trend analysis
- Department performance tracking

### Gathering Service

**Combined Calendar Management**

- Meeting and review aggregation
- Timeline organization
- Status-based filtering
- Monthly view generation

### Page Service

**Page Data Aggregation**

- Admin dashboard data compilation
- Employee profile data assembly
- Leave overview generation
- Employee management data collection

### Email Service

**Email Communications**

- Email template management
- Notification dispatch
- Test email functionality
- Email delivery tracking

### Image Service

**Image Processing**

- Image upload handling
- Profile picture management
- Image storage integration
- Image deletion and cleanup

### Integration Services

**External Services**

- Google OAuth integration
- Email service provider integration
- Image storage service integration

## Development Process

### Implementation Process

We followed an agile development approach with test-driven development, focusing on delivering features incrementally:

1. **Planning & Setup**

   - Set up ASP.NET Core backend with PostgreSQL
   - Implemented basic authentication and user management
   - Created initial database schema

2. **Core Features**

   - Admin & Employee Dashboards
   - Employee management system
   - Leave request system
   - Performance reviews

3. **Integration & Enhancement**
   - Google OAuth implementation
   - Email 2-factor authentication
   - Equipment Tracking
   - Meeting Requests (By Employee)
   -

### Highlights

**Calendar System**

- Interactive calendar interface for admins
- Month view with daily meeting breakdowns
- Click-to-view meeting details
- Color-coded events (meetings, reviews)

**Google OAuth Integration**

- Seamless login with Google accounts
- Automatic profile creation
- Profile picture integration
- Email verification bypass for Google users

**Employee Management**

- Comprehensive employee profiles with attached equipment tracking, performance review rating calculation and intuitive management of leave.

### Challenges

**Google OAuth Complexity**

- Managing different OAuth flows for registration vs login
- Handling token refresh and expiration
- Maintaining session state with OAuth

**Database Evolution**

- Frequent schema changes due to client requirements
- Managing data migrations without loss
- Restructuring relationships between entities
- Maintaining data integrity during updates

### Future Implementation

**Multi-Company Support**

- Separate database schemas per company
- Company-specific configurations
- Isolated data storage
- Custom branding per company

**Enhanced Authentication**

- Invite-based registration system
- Admin-generated employee invitations
- Password reset functionality
- Two-factor authentication options

**Future Features**

- Document management system
- Payroll integration
- Mobile application
- Advanced reporting tools

## Deployment

### Docker Configuration

The application includes a comprehensive Docker setup for easy deployment:

```dockerfile
# Multi-stage build for optimized image size
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CoriCore/CoriCore.csproj", "CoriCore/"]
RUN dotnet restore "CoriCore/CoriCore.csproj"
COPY . .
WORKDIR "/src/CoriCore"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
EXPOSE 5001
ENTRYPOINT ["dotnet", "CoriCore.dll"]
```

**Docker Commands:**

```bash
# Build the Docker image
docker build -t coriander-backend .

# Run the container
docker run -p 5000:5000 -p 5001:5001 coriander-backend

# Run with environment variables
docker run -p 5000:5000 --env-file .env coriander-backend
```

### Production Deployment

**Render Deployment:**

- Automatic deployments from GitHub main branch
- Environment variable management through Render dashboard
- PostgreSQL database hosting on Render
- SSL certificate automatic provisioning
- Health check monitoring and automatic restarts

**Environment-Specific Configurations:**

```json
{
  "Production": {
    "Logging": {
      "LogLevel": {
        "Default": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "AllowedHosts": "coriander-backend.onrender.com"
  }
}
```

## API Documentation

### Swagger Integration

The API includes comprehensive Swagger/OpenAPI documentation:

- **Interactive Documentation**: Available at `/swagger` endpoint
- **Authentication Support**: JWT Bearer token testing interface
- **Request/Response Examples**: Complete examples for all endpoints
- **Schema Definitions**: Detailed model definitions and validation rules

**Swagger Configuration:**

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Coriander HR API",
        Version = "v1",
        Description = "REST API for Coriander HR Management System"
    });

    // JWT Authentication support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});
```

<!-- VIDEO DEMONSTRATION -->

### Video Demonstration

To see a run through of the full application, click below:

[View Demonstration](https://youtu.be/EvPt8mvmRxk)

See the [open issues](https://github.com/WolfOWI/coriander-backend/issues) for a list of proposed features and known issues.

## Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

**Development Guidelines:**

- Follow C# coding conventions and best practices
- Write comprehensive unit tests for new features
- Update API documentation for endpoint changes
- Ensure backward compatibility where possible
- Include migration scripts for database changes

## Authors

- **Iné Smith** - _Lead UI/UX Designer_ - [inesmith](https://github.com/inesmith)
- **Kayla Posthumus** - _Lead Frontend Developer_ - [KaylaPosthumusOW](https://github.com/KaylaPosthumusOW)
- **Ruan Klopper** - _Lead Backend Developer_ - [Ruan-Klopper](https://github.com/Ruan-Klopper)
- **Wolf Botha** - _Project Coordinator_ - [WolfOWI](https://github.com/WolfOWI)

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

- **Backend Team** - [97434628+WolfOWI@users.noreply.github.com](mailto:97434628+WolfOWI@users.noreply.github.com)
- **Project Repository** - https://github.com/WolfOWI/coriander-backend
- **Live API** - https://coriander-backend.onrender.com
- **Frontend Repository** - https://github.com/WolfOWI/coriander

## Acknowledgements

**Technologies and Frameworks**

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) - Web API framework
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - Object-relational mapping
- [PostgreSQL](https://www.postgresql.org/) - Database management system
- [JWT.io](https://jwt.io/) - JSON Web Token implementation
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net) - Password hashing library
- [xUnit](https://xunit.net/) - Testing framework
- [Swagger/OpenAPI](https://swagger.io/) - API documentation

**External Services**

- [Render](https://render.com/) - Cloud hosting platform
- [MailKit](https://github.com/jstedfast/MailKit) - Email sending capabilities
- [Cloudinary](https://cloudinary.com/) - Image management and optimization

**Development Tools**

- [Visual Studio Code](https://code.visualstudio.com/) - Code editor
- [Postman](https://www.postman.com/) - API testing and documentation
- [Docker](https://www.docker.com/) - Containerisation platform
- [Git](https://git-scm.com/) - Version control system

<!-- Refer to https://shields.io/ for more information and options about the shield links at the top of the ReadMe file -->

[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=flat-square&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/nameonlinkedin/
[instagram-shield]: https://img.shields.io/badge/-Instagram-black.svg?style=flat-square&logo=instagram&colorB=555
[instagram-url]: https://www.instagram.com/instagram_handle/
[behance-shield]: https://img.shields.io/badge/-Behance-black.svg?style=flat-square&logo=behance&colorB=555
[behance-url]: https://www.behance.net/name-on-behance/
