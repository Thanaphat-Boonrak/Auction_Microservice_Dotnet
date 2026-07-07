"use client";
import { getData } from "@/app/actions/auctionActions";
import AuctionCard from "@/app/auctions/_components/AuctionCard";
import Filters from "@/app/auctions/_components/Filters";
import { useAuctionStore } from "@/app/hooks/useAuctionStore";
import { useParamsStore } from "@/app/hooks/useParamsStore";
import AppPagination from "@/app/shared/AppPagination";
import EmptyFilter from "@/app/shared/EmptyFilter";
import type { Auction } from "@/app/type";
import queryString from "query-string";
import { useEffect } from "react";
import { useShallow } from "zustand/shallow";

export default function Listing() {
  const data = useAuctionStore(
    useShallow((state) => ({
      auctions: state.auctions,
      totalCount: state.totalCount,
      pageCount: state.pageCount,
    })),
  );
  const setData = useAuctionStore((state) => state.setData);
  const params = useParamsStore(
    useShallow((state) => ({
      pageNumber: state.pageNumber,
      pageSize: state.pageSize,
      searchTerm: state.searchTerm,
      orderBy: state.orderBy,
      filterBy: state.filterBy,
      seller: state.seller,
      winner: state.winner,
    })),
  );

  const setParams = useParamsStore((state) => state.setParams);

  const url = queryString.stringifyUrl(
    { url: "", query: params },
    { skipEmptyString: true },
  );

  useEffect(() => {
    getData(url).then((data) => {
      setData(data);
    });
  }, [url, setData]);

  if (!data) return <h3>Loading ...</h3>;

  return (
    <>
      <Filters />
      {data.totalCount === 0 ? (
        <EmptyFilter showReset />
      ) : (
        <div className="grid grid-cols-3 gap-6 pb-5">
          {data.auctions &&
            data.auctions.map((auction: Auction) => (
              <AuctionCard key={auction.id} auction={auction} />
            ))}
        </div>
      )}
      {data.pageCount > 1 && (
        <div className="flex justify-center mt-4">
          <AppPagination
            pageChanged={(pageNumber: number) => setParams({ pageNumber })}
            currentPage={params.pageNumber}
            pageCount={data.pageCount}
          />
        </div>
      )}
    </>
  );
}
