from time import sleep
from network import Network

#importing required image
import base64


def main():
    with open("tmp.png", "rb") as imagefile:
        convert = base64.b64encode(imagefile.read())    
    n = Network()    
    p= str(n.getP().decode("utf-8"))
    print(p)  
    #n.only_send("/picture test2.jpeg".encode())   
    #n.wait() 
    #n.only_send((convert.decode("utf-8")).encode())  
    #print(len(convert))
    #n.only_send("}".encode())     
    n.only_send("/library".encode())
    stra="" 
    found=False
    while(1):
        k=n.wait()        
        print(k.decode("utf-8"))        
        stra+=k.decode("utf-8")
        if k.decode("utf-8").find("}") >= 0:               
            break     

main()