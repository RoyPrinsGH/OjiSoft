import React, { useState, useEffect } from "react";
import OjiSoftLogo from "../components/OjiSoftLogo";
import { getUser, logout } from "../utils/Auth";
import { User } from "oidc-client-ts";

const AccountPage: React.FC = () => {
    const [user, setUser] = useState<User | null>(null);

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
            <div className="flex flex-row justify-center">
                <pre className="text w-1/4 min-w-60">
                    {user?.profile.name}
                </pre>
            </div>
            <br />
            <br />
            <div className="flex flex-row justify-center gap-4">
                <button className="text button w-1/12 min-w-20" onClick={logout}>Log Out</button>
                <button className="text button w-1/12 min-w-20">Delete Account</button>
            </div>
        </div>
    );
}

export default AccountPage;