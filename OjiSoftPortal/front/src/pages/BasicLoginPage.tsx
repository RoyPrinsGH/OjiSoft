import OjiSoftLogo from "../components/OjiSoftLogo";

const BasicLoginPage: React.FC = () => {
    return (
        <div className="bg-black h-screen w-screen content-center">
            <OjiSoftLogo />
            <br />
            <div className="flex flex-row justify-center">
            <input className="text input w-1/4 min-w-60" spellCheck={false} placeholder="username"/>
            </div>
            <br />
            <div className="flex flex-row justify-center">
                <input className="text input w-1/4 min-w-60" spellCheck={false} type="password" placeholder="password"/>
            </div>
            <br />
            <div className="flex flex-row justify-center gap-4 pt-4">
                <button className="text button w-1/12 min-w-20">Log In</button>
                <button className="text button w-1/12 min-w-20">Register</button>
            </div>
        </div>
    );
};

export default BasicLoginPage;