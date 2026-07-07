import { auth } from "@/auth";
import { json } from "stream/consumers";

const baseUrl = process.env.API_URL

const get = async (url: string) => {
  const requestOptions = {
    method: "GET",
    headers: await getHeaders(),
  };

  const response = await fetch(baseUrl + url, requestOptions);
  return handleResponse(response);
};

const put = async (url: string, body: unknown) => {
  const requestOptions = {
    method: "PUT",
    headers: await getHeaders(),
    body: JSON.stringify(body),
  };

  const response = await fetch(baseUrl + url, requestOptions);
  return handleResponse(response);
};

const post = async (url: string, body: unknown) => {
  const requestOptions = {
    method: "POST",
    headers: await getHeaders(),
    body: JSON.stringify(body),
  };

  const response = await fetch(baseUrl + url, requestOptions);
  return handleResponse(response);
};

const del = async (url: string) => {
  const requestOptions = {
    method: "DELETE",
    headers: await getHeaders(),
  };

  const response = await fetch(baseUrl + url, requestOptions);
  return handleResponse(response);
};

const handleResponse = async (response: Response) => {
  const text = await response.text();
  let data;
  try {
    data = text ? JSON.parse(text) : null;
  } catch {
    data = text;
  }

  if (response.ok) {
    return data || { status: response.status, message: response.statusText };
  } else {
    const error = {
      status: response.status,
      message: typeof data === "string" ? data : response.statusText,
    };
    return { error };
  }
};

const getHeaders = async (): Promise<Headers> => {
  const session = await auth();
  const headers = new Headers();
  headers.set("Content-type", "application/json");
  if (session) {
    headers.set("Authorization", `Bearer ${session.accessToken}`);
  }
  return headers;
};

export const fetchWrapper = {
  get,
  post,
  del,
  put,
};
