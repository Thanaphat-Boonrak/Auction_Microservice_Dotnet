"use server";

import { fetchWrapper } from "@/app/lib/fetchWrapper";
import type { PageResult, Auction, bids } from "@/app/type";
import type { FieldValues } from "react-hook-form";

export async function getData(query: string): Promise<PageResult<Auction>> {
  return await fetchWrapper.get(`api/search${query}`);
}

export async function updateAuctionTest(): Promise<{
  status: number;
  message: string;
}> {
  const data = {
    mileage: Math.floor(Math.random() * 10000) + 1,
  };

  return await fetchWrapper.put(
      "api/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c",
      data,
  );
}

export async function createAuction(value: FieldValues) {
  return await fetchWrapper.post("api/auctions", value);
}

export async function getDetails(id: string) {
  return await fetchWrapper.get(`api/auctions/${id}`);
}

export async function updateAuction(data: FieldValues, id: string) {
  return await fetchWrapper.put(`api/auctions/${id}`, data);
}

export async function del(id: string) {
  return await fetchWrapper.del(`api/auctions/${id}`);
}

export async function getBidsForAuction(id: string): Promise<bids[]> {
  return await fetchWrapper.get(`api/bids/${id}`);
}

export async function placeBidForAuction(auctionId: string, amount: number) {
  return await fetchWrapper.post(
      `api/bids?auctionId=${auctionId}&amount=${amount}`,
      {},
  );
}