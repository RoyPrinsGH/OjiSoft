import OjiSoftLogo from "../components/OjiSoftLogo"
import React, { useEffect } from "react";
import { getUser, login } from "../utils/Auth";

const HomePage: React.FC = () => {
    useEffect(() => {
        getUser().then((user) => {
            if (user) {
                window.location.href = "/profile";
            }
        });
    }, []);

    return (
        <div className="bg-black h-screen w-screen content-center">
            <OjiSoftLogo />
            <br />
            <div className="flex flex-row justify-center">
                <pre className="text w-1/4 min-w-60">
                    {"Log in / register with:"}
                </pre>
            </div>
            <br />
            <div className="flex flex-row justify-center">
                <div className="flex flex-col w-1/6 min-w-60 gap-4">
                    <button className="text button" onClick={login}>My OjiSoft</button>
                </div>
            </div>
        </div>
    );

};

export default HomePage;