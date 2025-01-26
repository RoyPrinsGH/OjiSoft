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
            <div className="flex flex-row justify-center gap-4">
                <div className="flex flex-col w-1/12 gap-4 min-w-40 max-w-60">
                    <div className="flex flex-row justify-center">
                        <div className="flex flex-col justify-center border border-green-500 p-2.5 w-full">
                            <div className="flex flex-row justify-center border-b border-gray-500 mb-2">
                                <span className="text italic">
                                    Name
                                </span>
                            </div>
                            <div className="flex flex-row justify-center">
                                <span className="text-blue-500 whitespace-pre font-mono text-center">
                                    {user && user.profile.name}
                                </span>
                            </div>
                        </div>
                    </div>
                    <div className="flex flex-row justify-center">
                        <div className="flex flex-col justify-center border border-green-500 p-2.5 w-full">
                            <div className="flex flex-row justify-center border-b border-gray-500 mb-2">
                                <span className="text italic">
                                    Roles
                                </span>
                            </div>
                            {user && Object.values(user.profile.role).map(role => (
                                <div className="flex flex-row justify-center text-purple-500 whitespace-pre font-mono text-center">
                                    <span>{role as string}</span>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
                <div className="flex flex-col w-1/12 gap-4 min-w-40 max-w-60">
                    <div className="flex flex-row justify-center">
                        <div className="flex flex-col border border-green-500 p-2.5 w-full gap-2">
                            <div className="flex flex-row justify-center border-b border-gray-500 mb-2">
                                <span className="text italic">
                                    Actions
                                </span>
                            </div>
                            <div className="flex flex-row justify-center">
                                <button className="text button w-full">Edit Profile</button>
                            </div>
                            <div className="flex flex-row justify-center">
                                <button className="text button w-full" onClick={logout}>Log Out</button>
                            </div>
                            <div className="flex flex-row justify-center">
                                <button className="text button w-full">Delete Account</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
        </div>
    );
}

export default AccountPage;