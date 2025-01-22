import { useState } from "react";
import OjiSoftLogo from "../components/OjiSoftLogo";
import { login } from "../utils/Auth";

const BasicLoginPage: React.FC = () => {
    const [authStatus, setAuthStatus] = useState<boolean | null>(null);
    const [username, setUsername] = useState<string>("");
    const [password, setPassword] = useState<string>("");

    return (
        <div className="bg-black h-screen w-screen content-center">
            <OjiSoftLogo />
            <br />
            <div className="flex flex-row justify-center">
                <input className="text input w-1/4 min-w-60" spellCheck={false} placeholder="username" onChange={e => setUsername(e.target.value)}/>
            </div>
            <br />
            <div className="flex flex-row justify-center">
                <input className="text input w-1/4 min-w-60" spellCheck={false} type="password" placeholder="password" onChange={e => setPassword(e.target.value)}/>
            </div>
            <br />
            <div className="flex flex-row justify-center gap-4 pt-4">
                <button className="text button w-1/12 min-w-20" onClick={login}>Log In</button>
                <button className="text button w-1/12 min-w-20">Register</button>
            </div>
            <div className="flex flex-row justify-center pt-4">
                <pre className="text w-1/4 min-w-60">
                    {authStatus ? "Authenticated" : "Not authenticated"}
                </pre>
            </div>
        </div>
    );
};

export default BasicLoginPage;