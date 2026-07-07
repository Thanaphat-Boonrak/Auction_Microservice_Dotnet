"use client";
import Image from "next/image";
import { useState } from "react";

type Props = {
  image: string;
};

export default function CarImage({ image }: Props) {
  const [loading, setLoading] = useState<boolean>(true);

  return (
    <Image
      src={image}
      alt="Image of car"
      fill
      className={`object-cover duration-1000 ease-in-out ${loading ? "opacity-0 scale-1000" : "opacity-100 scale-100"}`}
      priority
      sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw"
      onLoad={() => setLoading(false)}
    />
  );
}
