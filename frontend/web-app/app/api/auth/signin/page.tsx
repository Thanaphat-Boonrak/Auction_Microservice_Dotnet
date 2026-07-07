import EmptyFilter from "@/app/shared/EmptyFilter";

export default async function SignIn({
  searchParams,
}: {
  searchParams: Promise<{ callbackUrl?: string }>;
}) {
  return (
    <div className="mt-15 ">
      <EmptyFilter
        title="You need to be logged in to do that"
        subtitle="Please click below to login"
        showLogin
        callbackUrl={(await searchParams).callbackUrl || "/"}
      />
    </div>
  );
}
