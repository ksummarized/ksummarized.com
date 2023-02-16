import axios from "../api/axios";

const useRefreshToken = () => {
  const refresh = async () => {
    const response = await axios.get("/auth/refresh-token", {
      withCredentials: true,
    });
    const localStorageUser = localStorage.getItem("user");
    let user = null;
    if (localStorageUser) {
      user = JSON.parse(localStorageUser);
      user.token = response.data.token;
      localStorage.setItem("user", JSON.stringify(user));
    }
    return response.data.token;
  };

  return refresh;
};

export default useRefreshToken;
