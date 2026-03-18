# حل سريع لمشكلة تسجيل الدخول 401

## المشكلة:
خطأ 401 Unauthorized عند تسجيل الدخول

## الحل السريع:

### 1. إنشاء حسابات User للأطباء والمرضى والموظفين:

افتح المتصفح أو Postman واذهب إلى:
```
POST http://localhost:5230/api/TestCredentials/ensure-users
```

سيعمل هذا على:
- إنشاء حسابات User لجميع الأطباء
- إنشاء حسابات User لجميع المرضى  
- إنشاء حسابات User لجميع الموظفين
- ربطهم ببعض

### 2. الحصول على بيانات تسجيل الدخول:

```
GET http://localhost:5230/api/TestCredentials/all
```

ستحصل على قائمة بجميع الحسابات مع:
- Email
- Password
- Role

### 3. بيانات تسجيل الدخول الافتراضية:

**Admin:**
- Email: `admin@hospital.com`
- Password: `Admin@123`
- Role: `Admin`

**Doctor:**
- Email: `ahmed.hassan@hospital.com`
- Password: `Doctor@123`
- Role: `Doctor`

**Patient:**
- Password: `Patient@123` (لجميع المرضى)
- Role: `Patient`

**Staff/Nurse:**
- Password: `Staff@123` (لجميع الموظفين)
- Role: `Nurse`

## ملاحظات:

1. تأكد أن الـ Backend يعمل على `http://localhost:5230`
2. إذا لم تكن الحسابات موجودة، استخدم `POST /api/TestCredentials/ensure-users`
3. رسائل الخطأ الآن أكثر وضوحاً وستخبرك بالضبط ما هي المشكلة

