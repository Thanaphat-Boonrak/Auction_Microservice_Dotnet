"use client";
import { useParamsStore } from "@/app/hooks/useParamsStore";
import Heading from "@/app/shared/Heading";
import { Button } from "flowbite-react";
import { signIn } from "next-auth/react";
import { HiOutlineLogin } from "react-icons/hi";

type Props = {
  title?: string;
  subtitle?: string;
  showReset?: boolean;
  showLogin?: boolean;
  callbackUrl?: string;
};

export default function EmptyFilter({
  title = "No matches for this filter",
  subtitle = "Try changing the filter or search term",
  showReset,
  callbackUrl,
  showLogin,
}: Props) {
  const reset = useParamsStore((state) => state.reset);

  return (
    <div className="flex flex-col gap-2 items-center  justify-center h-[40vh] shadow-lg">
      {showLogin && <HiOutlineLogin size={100} />}
      <Heading title={title} subtitle={subtitle} center />
      <div className="mt-4">
        {showReset && (
          <Button outline onClick={reset}>
            Remove filters
          </Button>
        )}

        {showLogin && (
          <Button outline onClick={() => signIn("id-server", { callbackUrl })}>
            Login
          </Button>
        )}
      </div>
    </div>
  );
}
