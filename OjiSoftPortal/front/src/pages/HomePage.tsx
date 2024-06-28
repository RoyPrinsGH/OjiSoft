import { Link } from "react-router-dom"
import OjiSoftLogo from "../components/OjiSoftLogo"

const HomePage: React.FC = () => {

    function redirectToGoogleAuth() {
        const clientID = '1083462064156-bkqafrusm9q8kqr7lnndro1s580ovman.apps.googleusercontent.com';
        const redirectURI = 'http://localhost:5173/query';
        const scope = 'openid';
        window.location.href = `https://accounts.google.com/o/oauth2/v2/auth?client_id=${clientID}&redirect_uri=${redirectURI}&scope=${scope}&response_type=code`;
    }

    function redirectToGitHubAuth() {
        const clientID = 'Ov23liX5DmMKhpGpH2md';
        const redirectURI = 'http://localhost:5173/query';
        const scope = 'read:user';
        window.location.href = `https://github.com/login/oauth/authorize?client_id=${clientID}&redirect_uri=${redirectURI}&scope=${scope}`;
    }

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
                    <Link to={"/login"} className="text button">Username and password</Link>
                    <button className="text button" onClick={redirectToGoogleAuth}>Google</button>
                    <button className="text button" onClick={redirectToGitHubAuth}>Github</button>
                </div>
            </div>
        </div>
    );

};

export default HomePage;