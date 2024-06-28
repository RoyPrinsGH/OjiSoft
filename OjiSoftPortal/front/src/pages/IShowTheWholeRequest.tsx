import React from 'react';
import { useSearchParams } from 'react-router-dom'

const IShowTheWholeRequest: React.FC = () => {
    const [searchParams, _] = useSearchParams()

    return (
        <div>
            <div>
                {Array.from(searchParams.entries()).map(([key, value]) => (
                    <div key={key}>
                        <span>{key}: </span>
                        <span>{value}</span>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default IShowTheWholeRequest;