import ReactDOM from 'react-dom/client'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import IndexPage from './pages/HomePage.tsx'
import './index.css'
import AccountPage from './pages/AccountPage.tsx'
import IShowTheWholeRequest from './pages/IShowTheWholeRequest.tsx'
import HandleLoginRedirect from './pages/HandleLoginRedirect.tsx'

ReactDOM.createRoot(document.getElementById('root')!).render(
    <Router>
      <Routes>
        <Route index path="/" Component={IndexPage}/>
        <Route path="/profile" Component={AccountPage}/>
        <Route path="/query" Component={IShowTheWholeRequest}/>
        <Route path="/handleloginredirect" Component={HandleLoginRedirect}/>
      </Routes>
    </Router>
)
