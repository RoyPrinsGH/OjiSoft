import OjiSoftLogo from "../components/OjiSoftLogo";

const AccountPage: React.FC = () => {
    return (
        <div className="bg-black h-screen w-screen content-center">
            <OjiSoftLogo />
            <br />
            <br />
            <div className="flex flex-row justify-center">
                <pre className="text w-1/4 min-w-60">
                    {`Username: test`}
                </pre>
            </div>
            <br />
            <br />
            <div className="flex flex-row justify-center gap-4">
                <button className="text button w-1/12 min-w-20">Log Out</button>
                <button className="text button w-1/12 min-w-20">Delete Account</button>
            </div>
        </div>
    );
}

export default AccountPage;