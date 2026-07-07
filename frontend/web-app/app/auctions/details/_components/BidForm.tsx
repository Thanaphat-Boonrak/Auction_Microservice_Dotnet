"use client";

import { placeBidForAuction } from "@/app/actions/auctionActions";
import { useBidStore } from "@/app/hooks/useBidStore";
import { useForm, type FieldValues } from "react-hook-form";
import toast from "react-hot-toast";
const INT_MAX = 2147483647;
type Props = {
  auctionId: string;
  highBid: number;
};

export default function BidForm({ auctionId, highBid }: Props) {
  const {
    register,
    handleSubmit,
    reset,
    setFocus,
    formState: { isSubmitting },
  } = useForm();
  const addBid = useBidStore((state) => state.addBids);
  const onSubmit = async (data: FieldValues) => {
    // if (data.amount > INT_MAX) {
    //   toast.error(`Can not bid over 2,000,000,000`);
    //   return;
    // }

    try {
      const bid = await placeBidForAuction(auctionId, data.amount);
      if (bid.error) {
        throw bid.error;
      }
      addBid(bid);
      setFocus("amount");
      reset();
    } catch (err) {
      const error = err as { status: string; message: string };
      toast.error(error.message ?? "Something went wrong");
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="flex items-center border-2 rounded-lg py-2"
    >
      <input
        {...register("amount", { valueAsNumber: true })}
        type="number"
        className="input-custom"
        disabled={isSubmitting}
        placeholder={`Enter your bid (minimum bid is $${highBid + 1})`}
      />
    </form>
  );
}
