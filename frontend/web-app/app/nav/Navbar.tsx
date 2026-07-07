"use client"
import UserActions from "@/app/shared/UserActions";
import LoginButton from "@/app/nav/LoginButton";
import Logo from "@/app/nav/Logo";
import Search from "@/app/nav/Search";
import { useSession } from "next-auth/react";
export default  function Navbar() {
  const session = useSession()
  return (
    <header className="sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-800 shadow-md">
      <Logo />
      <Search />

      {session.data?.user ? <UserActions user={session.data?.user} /> : <LoginButton />}
    </header>
  );
}
