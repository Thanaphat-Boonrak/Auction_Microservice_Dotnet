import AuctionForm from "@/app/auctions/create/_components/AuctionForm";
import Heading from "@/app/shared/Heading";

export default function page() {
  return (
    <div className="mx-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg">
      <Heading
        title={"Sell you car!!"}
        subtitle="Please enter the details of your car"
      />
      <AuctionForm />
    </div>
  );
}
