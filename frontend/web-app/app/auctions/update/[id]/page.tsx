import { getDetails } from "@/app/actions/auctionActions";
import AuctionForm from "@/app/auctions/create/_components/AuctionForm";
import Heading from "@/app/shared/Heading";

export default async function Update({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const data = await getDetails(id);

  return (
    <div className="mx-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg">
      <Heading
        title="Update your auction"
        subtitle="Please update the details of your car (only these auction properties can be updated)"
      />
      <AuctionForm auction={data} />
    </div>
  );
}
