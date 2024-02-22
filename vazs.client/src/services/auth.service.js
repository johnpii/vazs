// import axios from "axios";

// export const register = async(email, password, username) => {
//     axios
//       .post("Account/Regist", {
//         email,
//         password,
//         username,
//       })
//       .then((response) => {
//         console.log(JSON.stringify(response.data));
//       })
//       .catch((error) => {
//         console.log(error);
//       });
// }
// export const login = async(email, password) => {
//     axios
//       .post("/Account/Login", {
//         email,
//         password
//       })
//       .then((response) => {
//         console.log(JSON.stringify(response.data));
//       })
//       .catch((error) => {
//         console.log(error);
//       });
// }

const requestOptions = {
  method: "POST",
  credentials: "include",
};

export const login = (email, password) => {
  fetch(
    `/api/login?email=${email}&password=${password}`,
    requestOptions
  )
    .then((response) => response.text())
    .then((result) => console.log(result))
    .catch((error) => console.error(error));
}
export const register = (email, password, username) => {
  fetch(
    `https://localhost:7251/Account/Regist?email=${email}&password=${password}&username=${username}`,
    { method: "POST" }
  )
    .then((response) => response.text())
    .then((result) => console.log(result))
    .catch((error) => console.error(error));
};