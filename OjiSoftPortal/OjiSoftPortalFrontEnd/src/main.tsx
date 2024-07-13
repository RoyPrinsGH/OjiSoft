import React from 'react'
import ReactDOM from 'react-dom/client'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import IndexPage from './pages/HomePage.tsx'
import BasicLoginPage from './pages/BasicLoginPage.tsx'
import './index.css'
import AccountPage from './pages/AccountPage.tsx'
import IShowTheWholeRequest from './pages/IShowTheWholeRequest.tsx'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <Router>
      <Routes>
        <Route index path="/" Component={IndexPage}/>
        <Route path="/login" Component={BasicLoginPage}/>
        <Route path="/profile" Component={AccountPage}/>
        <Route path="/query" Component={IShowTheWholeRequest}/>
      </Routes>
    </Router>
  </React.StrictMode>,
)
