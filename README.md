# Secure Access Management System (SAMS) — Enterprise-Grade Showcase Project

## Overview

**SAMS** is a robust, secure access management solution designed as a **Backend-For-Frontend (BFF)** architecture to manage users, roles, policies, and applications (dashboards) across distinct systems with strong data isolation and policy-driven access control.

This project demonstrates real-world enterprise capabilities, including fine-grained authorization, role segregation, and secure multi-application governance — making it highly relevant for organizations seeking scalable and maintainable access control solutions.

---

## Architectural Highlights

### Backend-For-Frontend (BFF) Pattern

* The **Frontend UI** acts as a BFF layer, providing a seamless, secure management interface to internal users while mediating all interactions with the **Backend API**.
* This separation ensures that the frontend handles user authentication, authorization, and policy enforcement locally — and forwards only authorized, token-authenticated requests to the backend.
* This approach boosts security by minimizing direct backend exposure and enables centralized UI-specific logic and policy enforcement.

### Separate Databases for Clear Data Boundaries

* **Frontend Database:**

  * Stores all frontend user accounts, roles, policies, and role-policy mappings.
  * Supports dynamic, runtime management of access rights and user lifecycle within the frontend UI.

* **Backend Database:**

  * Exclusively stores dashboard/application users and configurations managed by SuperAdmins.
  * Backend API exposes tightly controlled endpoints consumed by the frontend BFF and authorized services.

---

## Role-Based and Policy-Driven Access Control

SAMS implements a granular **Policy-Based Access Control (PBAC)** system mapped to roles, enabling precise control over what users can see and do:

| Role           | Access Permissions Summary                                                                                                                                                                                                           |
| -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **SuperAdmin** | - Create, view, disable frontend users<br>- Create/delete policies and roles<br>- Assign/unassign policies<br>- Full backend dashboard (application) creation and management                                                         |
| **Admin**      | - Create and manage frontend users<br>- Create/delete frontend policies and roles<br>- Assign/unassign policies<br>- **Cannot create or manage backend dashboards** (enforced by backend authorization and frontend UI restrictions) |
| **Developer**  | - View frontend users<br>- Create frontend users<br>- Create/delete frontend policies and roles<br>- Assign/unassign policies                                                                                                        |
| **QA**         | - View frontend users only                                                                                                                                                                                                           |

### Enforcement

* **Backend API** strictly enforces role-based policies; only SuperAdmins can access dashboard creation endpoints.
* **Frontend UI** dynamically renders UI elements and restricts access based on logged-in user policies.
* Unauthorized access attempts redirect users to a custom Unauthorized view, ensuring clear feedback and secure navigation.

---

## Key Features

* **Secure Authentication & Authorization:**
  Frontend users authenticate against the frontend database; backend dashboard users are managed separately. Tokens and cookies secure communications between frontend and backend.

* **Dynamic Policy Management:**
  Admins and SuperAdmins manage policies and roles at runtime, allowing the system to adapt without redeployment.

* **Fine-Grained Access Controls:**
  Policies govern every actionable endpoint and UI element, preventing privilege escalation.

* **Audit Logging:**
  Detailed logs capture key actions such as user creation, dashboard provisioning, and access violations.

* **Multi-Application Segregation:**
  Clear separation of concerns between frontend management and backend dashboard services supports maintainability and compliance.

---

## Technology Stack

* ASP.NET Core 8 (API and MVC)
* Entity Framework Core (with separate SQL Server databases)
* JWT Authentication & Cookie-based session management
* Policy-Based Authorization
* Serilog for structured logging
* Tailwind CSS & Razor Views for responsive UI

---

## Setup Instructions

### Prerequisites

* .NET 8 SDK
* SQL Server instance(s) for frontend and backend databases
* Visual Studio 2022 / VS Code

### Installation

1. Clone the repository:

```bash
git clone https://github.com/your-org/sams.git
cd sams
```

2. Configure connection strings in `appsettings.json`:

* Frontend database connection
* Backend database connection

// Please look at appsettings.development.json and change marked areas as desired in order to ensure the system fucntions.
//These configurations should be done specifically in appsettings.json

3. Apply migrations separately on both databases to create schema.

4. Run the frontend and backend projects.

5. Use seeded SuperAdmin credentials to login and begin management.

---

## Usage Scenarios

* **SuperAdmin:** Full system administration, including backend dashboard provisioning and user/policy lifecycle management.
* **Admin:** Frontend user and policy management with no backend dashboard creation rights.
* **Developer:** User and policy creation with limited scope to enhance workflow.
* **QA:** Read-only user access to support auditing and validation.

---

## Security & Compliance

* Enforced **least privilege** principles across all operations.
* All sensitive actions require valid tokens issued via secure authentication flows.
* Unauthorized access attempts are logged and redirected for security auditing.

---

## Future Enhancements

* Full multitenancy support for hosting multiple organizations with data isolation.
* Integration with external identity providers (e.g., Azure AD, Okta).
* Advanced audit reporting and alerting capabilities.
* API rate limiting and security hardening.

---

## Contributing

Contributions are welcome via issues or pull requests. Please adhere to coding and security standards.

---

## License

MIT © Derrek Kinyanjui

---


// Please Note. Don't judge the frontend too harshly I am a backend Developer 😅😅😅
