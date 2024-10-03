// CustomerFooter.js
import React from 'react';
import SocialIcons from './SocialIcons';
import FooterColumn from './FooterColumn';
import SubscribeForm from './SubscribeForm';
import DownloadApp from './DownloadApp';

const CustomerFooter = () => {
    const supportContent = [
        { text: '111, ShopSilo Head Office, CH 01, Chennai.', link: 'https://maps.app.goo.gl/CKJWExKNcDWycr8d6' },
        { text: 'support@shopsilo.com', link: 'mailto:support@shopsilo.com' },
        { text: '+91 12345 67890', link: 'tel:+911234567890' }
    ];

    const accountContent = [
        { text: 'My Account', link: '/customer/account' },
        { text: 'Login / Register', link: '/customer/login' },
        { text: 'Cart', link: '/customer/cart' },
        { text: 'Wishlist', link: '/customer/wishlist' },
        { text: 'Shop', link: '/customer/shop' }
    ];

    const quickLinkContent = [
        { text: 'Privacy Policy', link: '/customer/privacy' },
        { text: 'Terms Of Use', link: '/customer/terms' },
        { text: 'FAQ', link: '/customer/faq' },
        { text: 'Contact', link: '/customer/contact' }
    ];

    return (
        <footer className="flex overflow-hidden flex-col justify-end pt-20 pb-6 bg-black">
            <div className="flex flex-wrap gap-10 justify-center items-start self-center max-md:max-w-full">
                <div className="flex flex-col text-neutral-50 w-[217px]">
                    <div className="flex flex-col self-start">
                        <div className="flex flex-col max-w-full whitespace-nowrap w-[118px]">
                            <h2 className="w-full text-2xl font-bold tracking-wider leading-none">
                                Exclusive
                            </h2>
                            <h3 className="mt-6 text-xl font-medium leading-snug">
                                Subscribe
                            </h3>
                        </div>
                        <p className="mt-6 text-base">Subscribe to our newsletter :)</p>
                    </div>
                    <SubscribeForm />
                </div>

                <FooterColumn title="Support" content={supportContent} />
                <FooterColumn title="Account" content={accountContent} />
                <FooterColumn title="Quick Link" content={quickLinkContent} />

                <DownloadApp />
            </div>

            <div className="flex flex-col items-center mt-16 w-full max-md:mt-10 max-md:max-w-full">
                <div className="flex flex-col w-full max-md:max-w-full">
                    <hr className="w-full bg-white border border-white border-solid opacity-40 min-h-[1px] max-md:max-w-full" />
                </div>
                <div className="flex gap-3 items-center mt-4 text-base text-white">
                    <div className="flex gap-1.5 items-center self-stretch my-auto min-w-[240px]">
                        <img loading="lazy" src="https://cdn.builder.io/api/v1/image/assets/TEMP/31106ccf7f9b47cd488fefba20d096b1d7303864f7c967b7ee43968ac28ce833?placeholderIfAbsent=true&apiKey=5fb2bd266d7c488887aa57ab2a82e6c8" className="object-contain shrink-0 self-stretch my-auto w-5 aspect-square" alt="" />
                        <p className="self-stretch my-auto">
                            Copyright ShopSilo 2024. All right reserved
                        </p>
                    </div>
                </div>
            </div>
        </footer>
    );
}

export default CustomerFooter;
