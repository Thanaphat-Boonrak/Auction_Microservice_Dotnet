"use client";

import { createAuction, updateAuction } from "@/app/actions/auctionActions";
import DateInput from "@/app/auctions/_components/DateInput";
import Input from "@/app/auctions/_components/Input";
import type { Auction } from "@/app/type";
import { error } from "console";
import { Button, Spinner } from "flowbite-react";
import { usePathname, useRouter } from "next/navigation";
import { useEffect } from "react";
import { useForm, type FieldValues } from "react-hook-form";
import toast from "react-hot-toast";

type Props = {
  auction?: Auction;
};

export default function AuctionForm({ auction }: Props) {
  const pathname = usePathname();
  const router = useRouter();
  const {
    control,
    handleSubmit,
    setFocus,
    reset,
    formState: { isSubmitting, isValid, isDirty },
  } = useForm({ mode: "onTouched" });

  useEffect(() => {
    if (auction) {
      const { make, model, color, mileage, year } = auction;
      reset({ make, model, color, mileage, year });
    }
    setFocus("make");
  }, [setFocus, auction, reset]);

  const onSubmit = async (data: FieldValues) => {
    try {
      let id = "";
      let response;
      if (pathname === "/auctions/create") {
        response = await createAuction(data);
        id = response.id;
      } else {
        if (auction) {
          id = auction.id;
          response = await updateAuction(data, id);
        }
      }

      if (response.error) throw response.error;

      router.push(`/auctions/details/${id}`);
    } catch (err) {
      if (typeof err === "object" && err !== null) {
        const error = err as { status?: string; message?: string };
        toast.error(`${error.status}: ${error.message}`);
      }
    }
  };

  return (
    <form className="flex flex-col mt-3" onSubmit={handleSubmit(onSubmit)}>
      <Input
        label={"Make"}
        name={"make"}
        control={control}
        rules={{ required: "Make is Required" }}
      />
      <Input
        label={"Model"}
        name={"model"}
        control={control}
        rules={{ required: "Model is Required" }}
      />
      <Input
        label={"Color"}
        name={"color"}
        control={control}
        rules={{ required: "Color is Required" }}
      />
      <div className="grid grid-cols-2 gap-3">
        <Input
          label={"Year"}
          name={"year"}
          control={control}
          type="number"
          rules={{ required: "Year is Required" }}
        />
        <Input
          label={"Mileage"}
          name={"mileage"}
          control={control}
          type="number"
          rules={{ required: "Mileage is Required" }}
        />
      </div>

      {pathname === "/auctions/create" && (
        <>
          <Input
            label={"Image Url"}
            name={"imageUrl"}
            control={control}
            rules={{ required: "Image Url is Required" }}
          />

          <div className="grid grid-cols-2 gap-3">
            <Input
              label={"Reserved Price (enter 0 if no reserve)"}
              name={"reservePrice"}
              control={control}
              type="number"
              rules={{ required: "Reserve price is Required" }}
            />
            <DateInput
              label={"Auction End"}
              name={"auctionEnd"}
              control={control}
              showTimeSelect
              dateFormat={"dd MMMM yyyy h:mm a"}
              rules={{ required: "Auction end date is Required" }}
            />
          </div>
        </>
      )}

      <div className="flex justify-between">
        <Button color="alternative" onClick={() => router.push("/")}>
          Cancel
        </Button>
        <Button
          outline
          color="green"
          type="submit"
          disabled={!isValid || !isDirty}
        >
          {isSubmitting && <Spinner size="sm" />}
          Submit
        </Button>
      </div>
    </form>
  );
}
