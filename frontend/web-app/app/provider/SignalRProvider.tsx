"use client";
import { getDetails } from "@/app/actions/auctionActions";
import AuctionCreatedToast from "@/app/auctions/_components/AuctionCreatedToast";
import AuctionFinishedToast from "@/app/auctions/_components/AuctionFinishedToast";
import { useAuctionStore } from "@/app/hooks/useAuctionStore";
import { useBidStore } from "@/app/hooks/useBidStore";
import type { Auction, AuctionFinished, bids } from "@/app/type";
import { HubConnectionBuilder, type HubConnection } from "@microsoft/signalr";
import { useSession } from "next-auth/react";
import { useParams } from "next/navigation";
import { useCallback, useEffect, useRef, type ReactNode } from "react";
import toast from "react-hot-toast";

type Props = {
  children: ReactNode;
};
export default function SignalRProvider({ children }: Props) {
  const session = useSession();
  const user = session.data?.user;
  const connection = useRef<HubConnection | null>(null);
  const setCurrentPrice = useAuctionStore((state) => state.setCurrentPrice);
  const addBid = useBidStore((state) => state.addBids);
  const params = useParams<{ id: string }>();
  const handleAuctionFinshed = useCallback(
    (finishedAuction: AuctionFinished) => {
      const auction = getDetails(finishedAuction.auctionId);
      return toast.promise(
        auction,
        {
          loading: "Loading",
          success: (auction) => (
            <AuctionFinishedToast
              finishedAuction={finishedAuction}
              auction={auction}
            />
          ),
          error: (err) => "Auction finished",
        },
        { success: { duration: 10000, icon: null } },
      );
    },
    [],
  );
  const handleAuctionCreated = useCallback(
    (auction: Auction) => {
      if (user?.username !== auction.seller) {
        return toast(<AuctionCreatedToast auction={auction} />, {
          duration: 10000,
        });
      }
    },
    [user?.username],
  );

  const handleBidPlaced = useCallback(
    (bid: bids) => {
      if (bid.bidStatus.includes("Accepted")) {
        setCurrentPrice(bid.auctionId, bid.amount);
      }
      if (params.id === bid.auctionId) {
        addBid(bid);
      }
    },
    [setCurrentPrice, addBid, params],
  );

  useEffect(() => {
    if (!connection.current) {
      connection.current = new HubConnectionBuilder()
        .withUrl(process.env.NEXT_PUBLIC_NOTIFY_URL!)
        .withAutomaticReconnect()
        .build();

      connection.current
        .start()
        .then(() => console.log("Connected to notification hub"))
        .catch((err) => console.log(err));

      connection.current.on("BidPlaced", handleBidPlaced);
      connection.current.on("AuctionCreated", handleAuctionCreated);
      connection.current.on("AuctionFinished", handleAuctionFinshed);
    }
  }, [
    setCurrentPrice,
    handleBidPlaced,
    handleAuctionCreated,
    handleAuctionFinshed,
  ]);
  return children;
}
