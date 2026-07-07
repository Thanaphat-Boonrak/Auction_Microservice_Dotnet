import { getDetails } from "@/app/actions/auctionActions";
import { getCurrentUser } from "@/app/actions/authAuctions";
import CarImage from "@/app/auctions/_components/CarImage";
import CountdownTimer from "@/app/auctions/_components/CountdownTimer";
import BidList from "@/app/auctions/details/_components/BidList";
import DeleteButton from "@/app/auctions/details/_components/DeleteButton";
import DetailedSpecs from "@/app/auctions/details/_components/DetailedSpecs";
import EditButton from "@/app/auctions/details/_components/EditButton";
import Heading from "@/app/shared/Heading";
export default async function page({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const data = await getDetails(id);
  const user = await getCurrentUser();
  return (
    <>
      <div className="flex justify-between">
        <div className="flex items-center gap-3">
          <Heading title={`${data.make} ${data.model}`} />
          {user?.username === data.seller && (
            <>
              <EditButton id={data.id} />
              <DeleteButton id={data.id} />
            </>
          )}
        </div>

        <div className="flex gap-3">
          <h3 className="text-2xl font-semibold">Time remaining:</h3>
          <CountdownTimer auctionEnd={data.auctionEnd} />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-6 mt-3">
        <div className="relative w-full bg-gray-200 aspect-[4/3] rounded-lg overflow-hidden">
          <CarImage image={data.imageUrl} />
        </div>
        <BidList user={user} auction={data} />
      </div>
      <div className="mt-3 grid grid-cols-1 rounded-lg pb-5">
        <DetailedSpecs auction={data} />
      </div>
    </>
  );
}
