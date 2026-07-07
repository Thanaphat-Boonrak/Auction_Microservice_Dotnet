import type { bids } from "@/app/type";
import { create } from "zustand";

type State = {
  bids: bids[];
  open: boolean;
};

type Actions = {
  setBids: (bids: bids[]) => void;
  addBids: (bid: bids) => void;
  setOpen: (value: boolean) => void;
};

export const useBidStore = create<State & Actions>((set) => ({
  bids: [],
  open: true,
  setBids: (bids: bids[]) => {
    set(() => ({
      bids,
    }));
  },
  addBids: (bid: bids) => {
    set((state) => ({
      bids: !state.bids.find((x) => x.id === bid.id)
        ? [bid, ...state.bids]
        : [...state.bids],
    }));
  },
  setOpen: (value: boolean) => {
    set(() => ({
      open: value,
    }));
  },
}));
