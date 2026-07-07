"use client";

import { useParamsStore } from "@/app/hooks/useParamsStore";
import { Dropdown, DropdownDivider, DropdownItem } from "flowbite-react";
import type { User } from "next-auth";
import { signOut } from "next-auth/react";
import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import path from "path";
import { AiFillCar, AiFillTrophy, AiOutlineLogout } from "react-icons/ai";
import { HiCog, HiUser } from "react-icons/hi";

type Props = {
  user: User;
};

export default function UserActions({ user }: Props) {
  const params = useParamsStore((state) => state.setParams);
  const reset = useParamsStore((state) => state.reset);
  const router = useRouter();
  const pathname = usePathname();
  return (
    <Dropdown inline label={`Welcome ${user.name}`} className="cursor-pointer">
      <DropdownItem
        icon={HiUser}
        onClick={() => {
          reset();
          if (pathname !== "/") router.push("/");
          params({ winner: undefined, seller: user.username });
        }}
      >
        My auctions
      </DropdownItem>
      <DropdownItem
        icon={AiFillTrophy}
        onClick={() => {
          reset();
          if (pathname !== "/") router.push("/");
          params({ winner: user.username, seller: undefined });
        }}
      >
        Auctions won
      </DropdownItem>

      <DropdownItem icon={AiFillCar}>
        <Link href={"/auctions/create"}>Sell my car</Link>
      </DropdownItem>

      <DropdownItem icon={HiCog}>
        <Link href={"/session"}>Session (dev only)</Link>
      </DropdownItem>
      <DropdownDivider />
      <DropdownItem
        icon={AiOutlineLogout}
        onClick={() => signOut({ redirectTo: "/" })}
      >
        Sign Out
      </DropdownItem>
    </Dropdown>
  );
}
