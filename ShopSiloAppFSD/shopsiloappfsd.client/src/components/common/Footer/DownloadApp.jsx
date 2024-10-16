import React from 'react';
import SocialIcons from './SocialIcons';

const DownloadApp = () => {
    return (
        <div className="flex flex-col">
            <div className="flex flex-col">
                <h3 className="text-xl font-medium leading-snug text-neutral-50">
                    Download App
                </h3>
                <div className="flex flex-col mt-6">
                    <p className="text-xs font-medium opacity-70 text-neutral-50">
                        Save $3 with App New User Only
                    </p>
                    <div className="flex gap-2 items-center mt-2">
                        <img loading="lazy" src="https://cdn.builder.io/api/v1/image/assets/TEMP/5440fda2301de4db42cb3b78ea9e454c8a6e45def7b4a78502c73f2e432adfbb?placeholderIfAbsent=true&apiKey=5fb2bd266d7c488887aa57ab2a82e6c8" className="object-contain shrink-0 self-stretch my-auto w-20 aspect-square" alt="QR Code" />
                        <div className="flex flex-col self-stretch my-auto w-[110px]">
                            <img loading="lazy" src="https://cdn.builder.io/api/v1/image/assets/TEMP/df59572eb8c812f019934d33ba52be08e637c623b1c2c4343e086e5a6869b3c4?placeholderIfAbsent=true&apiKey=5fb2bd266d7c488887aa57ab2a82e6c8" className="object-contain max-w-full aspect-[2.75] w-[110px]" alt="Download on App Store" />
                            <img loading="lazy" src="https://cdn.builder.io/api/v1/image/assets/TEMP/f5c8569b4d912f2f0d95c365e51f1f979a971234e5fa2a900f3797efb56a7b34?placeholderIfAbsent=true&apiKey=5fb2bd266d7c488887aa57ab2a82e6c8" className="object-contain mt-1 max-w-full aspect-[2.75] w-[110px]" alt="Get it on Google Play" />
                        </div>
                    </div>
                </div>
            </div>
            <SocialIcons />
        </div>
    );
}

export default DownloadApp;