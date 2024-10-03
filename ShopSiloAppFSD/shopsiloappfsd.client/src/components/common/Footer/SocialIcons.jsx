import React from 'react';

const SocialIcons = () => {
    const icons = [
        { src: "https://cdn.builder.io/api/v1/image/assets/TEMP/30ef1881c8ba382ad841b10dec22c4728e1ac56594cd8fd1b8fc54e4c0c91052?placeholderIfAbsent=true&apiKey=5fb2bd266d7c488887aa57ab2a82e6c8", alt: "Facebook" },
        { src: "https://cdn.builder.io/api/v1/image/assets/TEMP/8257474fffe306c0cc2a74f78732abb1f9425e53d7da84fbfdc5d6c50bf13925?placeholderIfAbsent=true&apiKey=5fb2bd266d7c488887aa57ab2a82e6c8", alt: "Twitter" },
        { src: "https://cdn.builder.io/api/v1/image/assets/TEMP/d0b67f52bb53b8c3b436cfc50867157e647c1cc5fa6f7cc373ba309b35f3d286?placeholderIfAbsent=true&apiKey=5fb2bd266d7c488887aa57ab2a82e6c8", alt: "Instagram" },
        { src: "https://cdn.builder.io/api/v1/image/assets/TEMP/017f03fd6271e23e94e189db8506f53ec67e53623ecac07e20b0cc0e64aa481c?placeholderIfAbsent=true&apiKey=5fb2bd266d7c488887aa57ab2a82e6c8", alt: "LinkedIn" }
    ];

    return (
        <div className="flex gap-6 items-start self-start mt-6">
            {icons.map((icon, index) => (
                <img
                    key={index}
                    loading="lazy"
                    src={icon.src}
                    className="object-contain shrink-0 w-6 aspect-square"
                    alt={icon.alt}
                />
            ))}
        </div>
    );
}

export default SocialIcons;