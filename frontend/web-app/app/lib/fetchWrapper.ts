import { auth } from "@/auth";

const rawBaseUrl = process.env.API_URL || "";
const baseUrl = rawBaseUrl.endsWith("/") ? rawBaseUrl.slice(0, -1) : rawBaseUrl;

const formatUrl = (path: string) => {
  const cleanPath = path.startsWith("/") ? path : `/${path}`;
  return `${baseUrl}${cleanPath}`;
};

const get = async (url: string) => {
  const requestOptions = {
    method: "GET",
    headers: await getHeaders(),
  };

  const response = await fetch(formatUrl(url), requestOptions);
  return handleResponse(response);
};

const put = async (url: string, body: unknown) => {
  const requestOptions = {
    method: "PUT",
    headers: await getHeaders(),
    body: JSON.stringify(body),
  };

  const response = await fetch(formatUrl(url), requestOptions);
  return handleResponse(response);
};

const post = async (url: string, body: unknown) => {
  const requestOptions = {
    method: "POST",
    headers: await getHeaders(),
    body: JSON.stringify(body),
  };

  const response = await fetch(formatUrl(url), requestOptions);
  return handleResponse(response);
};

const del = async (url: string) => {
  const requestOptions = {
    method: "DELETE",
    headers: await getHeaders(),
  };

  const response = await fetch(formatUrl(url), requestOptions);
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
    return data !== null && data !== undefined ? data : { status: response.status, message: response.statusText };
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
  if (session && 'accessToken' in session) {
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