import React, { useState, useEffect } from 'react';

const CountdownTimer = ({ endTime }) => {
    const calculateTimeLeft = () => {
        const difference = +new Date(endTime) - +new Date();
        let timeLeft = {};

        if (difference > 0) {
            timeLeft = {
                days: String(Math.floor(difference / (1000 * 60 * 60 * 24))).padStart(2, '0'),
                hours: String(Math.floor((difference / (1000 * 60 * 60)) % 24)).padStart(2, '0'),
                minutes: String(Math.floor((difference / 1000 / 60) % 60)).padStart(2, '0'),
                seconds: String(Math.floor((difference / 1000) % 60)).padStart(2, '0'),
            };
        }

        return timeLeft;
    };

    const [timeLeft, setTimeLeft] = useState(calculateTimeLeft());

    useEffect(() => {
        const timer = setInterval(() => {
            setTimeLeft(calculateTimeLeft());
        }, 1000);

        // Cleanup the interval on component unmount
        return () => clearInterval(timer);
    }, [endTime]);

    return (
        <div className="text-lg text-black px-4 py-2 rounded-md text-center">
            <div className="flex items-center justify-center space-x-2">
                {Object.entries(timeLeft).map(([key, value], index) => (
                    <div key={key} className="flex flex-col items-center">
                        <h5 className="text-xs w-12 text-left font-semibold">{key.charAt(0).toUpperCase() + key.slice(1)}</h5>
                        <span className="font-bold text-3xl">{value || '00'} {index < Object.keys(timeLeft).length - 1 && (
                            <span className="text-orange-500 text-3xl mx-1"> :</span>
                        )}</span>
                    </div>
                ))}
            </div>
            {Object.keys(timeLeft).length === 0 && <span className="block mt-2">Sale Ended</span>}
        </div>
    );
};

export default CountdownTimer;
