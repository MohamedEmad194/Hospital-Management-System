# اختبار Nursing Units - خطوات التشخيص

## 1. التحقق من وجود الجدول:
```
GET http://localhost:5230/api/NursingUnits/check-table
```

## 2. التحقق من البيانات:
```
GET http://localhost:5230/api/NursingUnits
```

## 3. تحميل البيانات (إذا كانت غير موجودة):
```
POST http://localhost:5230/api/NursingUnits/seed
```

## 4. التحقق من Frontend:
افتح Console في المتصفح وتحقق من:
- هل هناك أخطاء في Network tab?
- هل endpoint يتم استدعاؤه بشكل صحيح?

