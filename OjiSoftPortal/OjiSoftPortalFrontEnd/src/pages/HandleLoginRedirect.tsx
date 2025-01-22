import { useEffect } from "react";
import { userManager } from "../utils/OidcConfig";

const HandleLoginRedirect: React.FC = () => {
    useEffect(() => {
        async function handleRedirect() {
            try {
                await userManager.signinRedirectCallback();
                window.location.href = "/";
            } catch (error) {
                console.error("Error during login redirect callback:", error);
            }
        }
        handleRedirect();
    }, []);

    return (
        <div>
            <p>Redirecting...</p>
        </div>
    );
};

export default HandleLoginRedirect;