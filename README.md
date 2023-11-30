# EZYHIRE - Comprehensive Job Portal using C# .NET

## About

The project is a web-based job portal built using C# .NET and RESTful APIs. It serves as a digital bridge connecting job seekers and employers. The system features user authentication, profile management, and a powerful job search functionality. Job seekers can explore detailed job listings, apply seamlessly, and manage their resumes, while employers can post job opportunities and review applications efficiently. The project combines modern web technologies with a user-centric design to create a robust and user-friendly platform for job seekers and employers alike. It leverages RESTful APIs for efficient data exchange, ensuring smooth interactions between the frontend and backend. The project emphasizes data security, responsive design, and a user-friendly interface to enhance the user experience. The project also comprises of AES encryption for all user details and SHA256 hashing for hashing the secured password to ensure greater protective measure in protecting the user’s data.

## Technology Stack used

1. RazorPages
2. C#
3. ASP.NETCore
4. HTML
5. CSS
6. SQLITE

## Functionality used

1. **Authentication and User Management(Login/Signup)**:
This module handles user registration and login. It manages user profiles and authentication to ensure data security. This module allows users to signup if they are new to the portal, or login if they have already registered.

2. **Profile Management for JobSeekers**:
Job seekers can create, update and delete their profiles. They can upload and manage their resumes and other relevant information. All the changes are done by sending requests to the REST API server which makes the necessary changes in the database

3. **Search and Filtering**:
Includes a powerful search bar with filters to help job seekers find specific job listings based on criteria such as location, job type, and keywords.

4. **Trending Job Listing**:
All the available jobs are listed in the order of higher pay, better ratings of the company and other relevant details. The jobs are listed below as trending jobs based on these factors.

5. **Encryption**:
The portal comprises AES encryption which is one of the strongest encryption in today’s world to store all the user’s data in a secured format. This is encrypted using AES

6. **Hashing**:
For ensuring even better security, hashing is done using SHA256 algorithm so that it can never be never decrypted to plain text format and ensures better security.

## How to use this project

1. Download the zip file
2. Unzip it and open a terminal
3. Use the command 'dotnet run' or 'dotnet-watch run' to build the application
