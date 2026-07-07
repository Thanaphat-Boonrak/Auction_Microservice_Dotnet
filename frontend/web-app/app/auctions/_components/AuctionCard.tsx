import CarImage from "@/app/auctions/_components/CarImage";
import CountdownTimer from "@/app/auctions/_components/CountdownTimer";
import CurrentBid from "@/app/auctions/_components/CurrentBid";
import type { Auction } from "@/app/type";
import Link from "next/link";

type Props = {
  auction: Auction;
};

export default function AuctionCard({ auction }: Props) {
  return (
    <Link href={`/auctions/details/${auction.id}`}>
      <div className="w-full relative bg-gray-200 aspect-video rounded-lg overflow-hidden">
        <CarImage image={auction.imageUrl} />
        <div className="absolute bottom-2 left-2">
          <CountdownTimer auctionEnd={auction.auctionEnd} />
        </div>
        <div className="absolute bottom-2 right-2">
          <CurrentBid
            reservePrice={auction.reservePrice}
            amount={auction.currentHighBid}
          />
        </div>
      </div>

      <div className="flex justify-between items-center mt-4">
        <h3 className="text-gray-500">
          {auction.make} {auction.model}
        </h3>
        <p className="font-semibold text-sm">{auction.year}</p>
      </div>
    </Link>
  );
}
