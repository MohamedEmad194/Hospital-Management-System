import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import AdminRoute from './components/AdminRoute';
import StaffRoute from './components/StaffRoute';
import Layout from './components/Layout';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import NursingServices from './pages/NursingServices';
import './App.css';
import PatientsList from './pages/PatientsList';
import DoctorsList from './pages/DoctorsList';
import AppointmentsList from './pages/AppointmentsList';
import BillsList from './pages/BillsList';
import DepartmentsList from './pages/DepartmentsList';
import MedicinesList from './pages/MedicinesList';
import AddDoctor from './pages/AddDoctor';
import AddMedicine from './pages/AddMedicine';
import AddPatient from './pages/AddPatient';
import AddDepartment from './pages/AddDepartment';
import AddAppointment from './pages/AddAppointment';
import AddBill from './pages/AddBill';
import RoomsList from './pages/RoomsList';
import AddRoom from './pages/AddRoom';
import Chatbot from './pages/Chatbot';
import PatientReports from './pages/PatientReports';
import Payment from './pages/Payment';
import XRayAnalyzer from './pages/XRayAnalyzer';

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter
        future={{
          v7_startTransition: true,
          v7_relativeSplatPath: true,
        }}
      >
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/" element={<Layout />}>
            <Route index element={<Dashboard />} />
            <Route path="/nursing" element={<NursingServices />} />
            <Route element={<ProtectedRoute />}>
              <Route path="/chatbot" element={<Chatbot />} />
              <Route path="/appointments" element={<AppointmentsList />} />
              <Route path="/appointments/add" element={<AddAppointment />} />
              <Route path="/bills" element={<BillsList />} />
              <Route path="/bills/:id/payment" element={<Payment />} />
              <Route path="/reports" element={<PatientReports />} />
              <Route element={<StaffRoute />}>
                <Route path="/patients" element={<PatientsList />} />
                <Route path="/patients/add" element={<AddPatient />} />
                <Route path="/doctors" element={<DoctorsList />} />
                <Route path="/bills/add" element={<AddBill />} />
                <Route path="/departments" element={<DepartmentsList />} />
                <Route path="/medicines" element={<MedicinesList />} />
                <Route path="/medicines/add" element={<AddMedicine />} />
                <Route path="/rooms" element={<RoomsList />} />
                <Route path="/rooms/add" element={<AddRoom />} />
              </Route>
              <Route element={<AdminRoute />}>
                <Route path="/xray-ai" element={<XRayAnalyzer />} />
                <Route path="/doctors/add" element={<AddDoctor />} />
                <Route path="/departments/add" element={<AddDepartment />} />
              </Route>
            </Route>
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
