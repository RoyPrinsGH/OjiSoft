import { useEffect } from "react";
import OjiSoftLogo from "../components/OjiSoftLogo";
import { userManager } from "../utils/OidcConfig";

const HandleLoginRedirect: React.FC = () => {
    useEffect(() => {
        async function handleRedirect() {
            try {
                await userManager.signinCallback();
                window.location.href = "/";
            } catch (error) {
                console.error("Error during login redirect callback:", error);
            }
        }
        handleRedirect();
    }, []);

    return (
        <div className="bg-black h-screen w-screen content-center">
            <OjiSoftLogo />
            <br />
            <div className="flex flex-row justify-center gap-4 pt-4">
                <p className="text button w-1/12 min-w-20">Redirecting...</p>
            </div>
        </div>
    );
};

export default HandleLoginRedirect;