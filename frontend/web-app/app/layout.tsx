import type { Metadata } from "next";
import "./globals.css";
import Navbar from "@/app/nav/Navbar";
import ToasterProvider from "@/app/provider/ToasterProvider";
import SignalRProvider from "@/app/provider/SignalRProvider";
import { SessionProvider } from "next-auth/react";
export const dynamic = "force-dynamic";
export const metadata: Metadata = {
  title: "Christie",
  description: "Generate By Thanaphat2005",
};

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>
        <SessionProvider>
          <ToasterProvider />
          <Navbar />
          <main className="container mx-auto px-5 pt-10">
            <SignalRProvider>{children}</SignalRProvider>
          </main>
        </SessionProvider>
      </body>
    </html>
  );
}
