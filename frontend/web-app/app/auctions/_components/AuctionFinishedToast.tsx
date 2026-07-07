import { Auction, AuctionFinished } from "@/app/type";
import Image from "next/image";
import Link from "next/link";
type Props = {
  finishedAuction: AuctionFinished;
  auction: Auction;
};

export default function AuctionFinishedToast({
  finishedAuction,
  auction,
}: Props) {
  return (
    <Link
      href={`/auctions/details/${auction.id}`}
      className="flex flex-col items-center"
    >
      <div className="flex flex-row items-center gap-2">
        <Image
          src={auction.imageUrl}
          alt="Image of car"
          height={80}
          width={80}
          className="rounded-lg w-auto h-auto"
        />
        <div className="flex flex-col">
          <span>
            {finishedAuction.itemSold && finishedAuction.amount ? (<p>Congrats to {finishedAuction.winner} who has won this auction for {finishedAuction.amount}</p>) : "This item not selll"}
          </span>
        </div>
      </div>
    </Link>
  );
}
