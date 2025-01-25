import React, { useState, useEffect } from "react";
import OjiSoftLogo from "../components/OjiSoftLogo";
import { getUser, logout } from "../utils/Auth";

const AccountPage: React.FC = () => {
    const [user, setUser] = useState<any | null>(null);

    useEffect(() => {
        getUser().then((user) => {
            setUser(user);
        });
    }, []);

    return (
        <div className="bg-black h-screen w-screen content-center">
            <OjiSoftLogo />
            <br />
            <br />
            <div className="flex flex-col justify-center">
                <div className="flex flex-row justify-center mb-2">
                    <span className="text w-1/4 min-w-60 italic">
                        --- Name ---
                    </span>
                </div>
                <div className="flex flex-row justify-center mb-2">
                    <span className="text-blue-500 bg-black whitespace-pre font-mono text-center w-1/4 min-w-60">
                        {user && user.profile.name}
                    </span>
                </div>
                <br />
                <div className="flex flex-row justify-center mb-2">
                    <span className="text w-1/4 min-w-60 italic">
                        --- Roles ---
                    </span>
                </div>
                <div className="flex flex-row justify-center">
                    <div className="flex flex-col justify-center">
                        {user && Object.values(user.profile.role).map(role => (
                            <div className="text-purple-500 bg-black whitespace-pre font-mono text-center w-1/4 min-w-60">
                                {role as string}
                            </div>
                        ))}
                    </div>
                </div>
                <br />
                <div className="flex flex-row justify-center mb-2">
                    <span className="text w-1/4 min-w-60 italic">
                        --- Actions ---
                    </span>
                </div>
                <div className="flex flex-row justify-center gap-4">
                    <button className="text button w-1/12 min-w-20" onClick={logout}>Log Out</button>
                    <button className="text button w-1/12 min-w-20">Edit Profile</button>
                    <button className="text button w-1/12 min-w-20">Delete Account</button>
                </div>
            </div>
        </div>
    );
}

export default AccountPage;