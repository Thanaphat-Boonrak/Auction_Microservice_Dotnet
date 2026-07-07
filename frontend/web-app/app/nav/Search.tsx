"use client";

import { useParamsStore } from "@/app/hooks/useParamsStore";
import { FaSearch } from "react-icons/fa";

export default function Search() {
  const setParams = useParamsStore((state) => state.setParams);
  const searchTerm = useParamsStore((state) => state.searchTerm);

  return (
    <div className="flex w-[50%] items-center border-2 border-gray-300 rounded-full px-2 py-2 shadow-sm">
      <input
        value={searchTerm}
        onChange={(e) => setParams({ searchTerm: e.target.value })}
        type="text"
        placeholder="Search for cars by make, model or color"
        className="input-custom"
      />
      <button>
        <FaSearch
          size={34}
          className="bg-red-400 text-white rounded-full p-2 cursor-pointer"
        />
      </button>
    </div>
  );
}
