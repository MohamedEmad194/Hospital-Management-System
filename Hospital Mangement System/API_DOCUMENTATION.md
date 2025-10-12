# Hospital Management System API Documentation

## Overview
This is a comprehensive Hospital Management System API built with ASP.NET Core 8.0. The API provides endpoints for managing patients, doctors, appointments, medical records, prescriptions, bills, and more.

## Base URL
- Development: `https://localhost:7102` or `http://localhost:5230`
- Production: `https://your-domain.com`

## Authentication
The API uses JWT (JSON Web Token) for authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

## API Endpoints

### Authentication

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "nationalId": "1234567890",
  "dateOfBirth": "1990-01-01",
  "gender": "Male",
  "address": "123 Main St",
  "phoneNumber": "1234567890"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@hospital.com",
  "password": "Admin@123"
}
```

#### Change Password
```http
POST /api/auth/change-password
Authorization: Bearer <token>
Content-Type: application/json

{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword123!",
  "confirmNewPassword": "NewPassword123!"
}
```

### Patients

#### Get All Patients
```http
GET /api/patients
Authorization: Bearer <token>
```

#### Get Patient by ID
```http
GET /api/patients/{id}
Authorization: Bearer <token>
```

#### Create Patient
```http
POST /api/patients
Authorization: Bearer <token>
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith",
  "nationalId": "9876543210",
  "email": "jane.smith@example.com",
  "phoneNumber": "0987654321",
  "dateOfBirth": "1985-05-15",
  "gender": "Female",
  "address": "456 Oak Ave",
  "emergencyContactName": "John Smith",
  "emergencyContactPhone": "1111111111",
  "insuranceProvider": "Health Insurance Co",
  "insuranceNumber": "INS123456",
  "medicalHistory": "No known allergies",
  "allergies": "None",
  "currentMedications": "None"
}
```

#### Update Patient
```http
PUT /api/patients/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  "firstName": "Jane Updated",
  "phoneNumber": "0987654321",
  "address": "789 Pine St"
}
```

#### Delete Patient
```http
DELETE /api/patients/{id}
Authorization: Bearer <token>
```

### Doctors

#### Get All Doctors
```http
GET /api/doctors
Authorization: Bearer <token>
```

#### Get Doctor by ID
```http
GET /api/doctors/{id}
Authorization: Bearer <token>
```

#### Create Doctor
```http
POST /api/doctors
Authorization: Bearer <token>
Content-Type: application/json

{
  "firstName": "Dr. Ahmed",
  "lastName": "Hassan",
  "nationalId": "1111111111",
  "email": "ahmed.hassan@hospital.com",
  "phoneNumber": "2222222222",
  "dateOfBirth": "1980-03-20",
  "gender": "Male",
  "address": "Hospital Address",
  "licenseNumber": "LIC123456",
  "specialization": "Cardiology",
  "subSpecialization": "Interventional Cardiology",
  "yearsOfExperience": 15,
  "education": "MD in Cardiology",
  "certifications": "Board Certified Cardiologist",
  "languages": "Arabic, English",
  "consultationFee": 200.00,
  "workingHoursStart": "09:00:00",
  "workingHoursEnd": "17:00:00",
  "departmentId": 1
}
```

### Appointments

#### Get All Appointments
```http
GET /api/appointments
Authorization: Bearer <token>
```

#### Create Appointment
```http
POST /api/appointments
Authorization: Bearer <token>
Content-Type: application/json

{
  "appointmentDate": "2024-01-15",
  "appointmentTime": "10:00:00",
  "reason": "Regular checkup",
  "notes": "Patient has been feeling tired",
  "isFollowUp": false,
  "patientId": 1,
  "doctorId": 1,
  "roomId": 1
}
```

#### Cancel Appointment
```http
PUT /api/appointments/{id}/cancel
Authorization: Bearer <token>
```

#### Complete Appointment
```http
PUT /api/appointments/{id}/complete
Authorization: Bearer <token>
Content-Type: application/json

{
  "diagnosis": "General fatigue, recommend rest",
  "treatment": "Get adequate sleep, regular exercise"
}
```

### Medical Records

#### Get Medical Records by Patient
```http
GET /api/medicalrecords/patient/{patientId}
Authorization: Bearer <token>
```

#### Create Medical Record
```http
POST /api/medicalrecords
Authorization: Bearer <token>
Content-Type: application/json

{
  "recordDate": "2024-01-15",
  "recordType": "Consultation",
  "symptoms": "Fatigue, headache",
  "diagnosis": "Stress-related symptoms",
  "treatment": "Rest and relaxation",
  "prescription": "Paracetamol 500mg",
  "notes": "Patient advised to reduce stress",
  "vitalSigns": "BP: 120/80, HR: 72",
  "bloodPressure": "120/80",
  "temperature": "98.6°F",
  "heartRate": "72",
  "weight": "70kg",
  "height": "170cm",
  "patientId": 1,
  "doctorId": 1
}
```

### Prescriptions

#### Get Prescriptions by Patient
```http
GET /api/prescriptions/patient/{patientId}
Authorization: Bearer <token>
```

#### Create Prescription
```http
POST /api/prescriptions
Authorization: Bearer <token>
Content-Type: application/json

{
  "prescriptionDate": "2024-01-15",
  "validUntil": "2024-01-22",
  "instructions": "Take with food",
  "notes": "Complete the full course",
  "patientId": 1,
  "doctorId": 1,
  "prescriptionItems": [
    {
      "medicineName": "Paracetamol",
      "dosage": "500mg",
      "frequency": "Twice daily",
      "duration": "7 days",
      "instructions": "Take with food",
      "quantity": 14,
      "unitPrice": 5.00,
      "medicineId": 1
    }
  ]
}
```

#### Dispense Prescription
```http
PUT /api/prescriptions/{id}/dispense
Authorization: Bearer <token>
```

### Bills

#### Get Bills by Patient
```http
GET /api/bills/patient/{patientId}
Authorization: Bearer <token>
```

#### Create Bill
```http
POST /api/bills
Authorization: Bearer <token>
Content-Type: application/json

{
  "billDate": "2024-01-15",
  "dueDate": "2024-01-22",
  "notes": "Consultation and medication",
  "insuranceProvider": "Health Insurance Co",
  "insuranceNumber": "INS123456",
  "insuranceCoverage": 80.00,
  "patientId": 1,
  "billItems": [
    {
      "description": "Consultation Fee",
      "category": "Consultation",
      "quantity": 1,
      "unitPrice": 200.00,
      "notes": "Cardiology consultation"
    },
    {
      "description": "Paracetamol 500mg",
      "category": "Medicine",
      "quantity": 14,
      "unitPrice": 5.00,
      "notes": "Pain relief medication"
    }
  ]
}
```

#### Process Payment
```http
POST /api/bills/{id}/payment
Authorization: Bearer <token>
Content-Type: application/json

{
  "amount": 150.00,
  "paymentMethod": "Credit Card",
  "notes": "Payment processed successfully"
}
```

### Dashboard

#### Get Dashboard Statistics
```http
GET /api/dashboard/stats
Authorization: Bearer <token>
```

#### Get Recent Appointments
```http
GET /api/dashboard/recent-appointments?count=10
Authorization: Bearer <token>
```

#### Get Revenue Statistics
```http
GET /api/dashboard/revenue
Authorization: Bearer <token>
```

## Response Format

### Success Response
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "createdAt": "2024-01-15T10:00:00Z"
}
```

### Error Response
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": [
      "The Email field is required."
    ]
  }
}
```

## Status Codes

- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Rate Limiting

The API implements rate limiting to prevent abuse:
- 100 requests per minute per IP address
- 1000 requests per hour per authenticated user

## Pagination

For endpoints that return lists, pagination is supported:

```http
GET /api/patients?page=1&pageSize=10
```

Response includes pagination metadata:
```json
{
  "data": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 100,
  "totalPages": 10
}
```

## Filtering and Searching

Many endpoints support filtering and searching:

```http
GET /api/patients?search=john
GET /api/appointments?status=scheduled&doctorId=1
GET /api/medicines?lowStock=true
```

## Webhooks

The API supports webhooks for real-time notifications:
- Appointment created/updated/cancelled
- Payment processed
- Prescription dispensed

Configure webhooks in your account settings.
