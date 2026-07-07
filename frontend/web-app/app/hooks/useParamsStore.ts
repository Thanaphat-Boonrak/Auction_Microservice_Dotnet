import { stat } from "fs";
import { create } from "zustand";

interface State {
  pageNumber: number;
  pageSize: number;
  pageCount: number;
  searchTerm: string;
  orderBy: string;
  filterBy: string;
  seller?: string;
  winner?: string;
}

interface Actions {
  setParams: (params: Partial<State>) => void;
  reset: () => void;
}

const intialState: State = {
  pageNumber: 1,
  pageSize: 6,
  pageCount: 1,
  searchTerm: "",
  orderBy: "make",
  filterBy: "live",
  winner: undefined,
  seller: undefined,
};

export const useParamsStore = create<State & Actions>((set) => ({
  ...intialState,
  setParams: (newParams: Partial<State>) => {
    set((state) => {
      const newState = { ...state, ...newParams };
      if (
        newParams.searchTerm !== undefined ||
        newParams.pageSize !== undefined ||
        newParams.orderBy !== undefined ||
        newParams.filterBy !== undefined ||
        newParams.seller !== undefined ||
        newParams.winner !== undefined
      ) {
        newState.pageNumber = 1;
      }
      return newState;
    });
  },
  reset: () => {
    set(intialState);
  },
}));
