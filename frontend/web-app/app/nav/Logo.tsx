"use client";

import { useParamsStore } from "@/app/hooks/useParamsStore";
import { usePathname } from "next/navigation";
import { useRouter } from "next/navigation";
import { AiOutlineCar } from "react-icons/ai";

export default function Logo() {
  const reset = useParamsStore((state) => state.reset);
  const router = useRouter();
  const pathname = usePathname();
  function handleReset() {
    if (pathname !== "/") router.push("/");
    reset();
  }
  return (
    <div
      onClick={handleReset}
      className="flex items-center gap-2 text-3xl font-semibold text-red-600 cursor-pointer"
    >
      <AiOutlineCar size={34} />
      <div>Christie Auctions</div>
    </div>
  );
}
