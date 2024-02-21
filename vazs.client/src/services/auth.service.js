import axios from "axios";

export const register = async(email, password, username) => {
    try{
        const res = await axios.post(
        "http://localhost:7251/Account/Regist",
        {
            email,
            password,
            username
        },
        {
            withCredentials: true
        })  
        // console.log(res);
        // return { res: res, status: res.status };
    } catch (e) {
        console.log(e)
        // return { res: e, status: e.response.status };
    }  
}
export const login = async(email, password) => {
    try{
        axios.post(
            "http://localhost:7251/Account/Login",
        {
            email,
            password,
        },
        {
            withCredentials: true,
        })
        // console.log(res);
        // return { res: res, status: res.status };
    }catch(e){
        console.log(e)
    };
}