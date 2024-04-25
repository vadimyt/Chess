from time import sleep
from network import Network

#importing required image
import base64


def main():
    #with open("test1.jpeg", "rb") as imagefile:
        #convert = base64.b64encode(imagefile.read())    
    n = Network()    
    p= str(n.getP().decode("utf-8"))
    print(p)  
    #n.only_send("/picture test1.jpeg".encode())   
    #n.wait() 
    #n.only_send((convert.decode("utf-8")).encode())  
    #print(len(convert))
    #n.only_send("}".encode())     
    n.only_send("/getpicture 2".encode())
    stra="" 
    found=False
    while(1):
        k=n.wait()       
               
        stra+=k.decode("utf-8")
        if k.decode("utf-8").find("}") >= 0:            
            n.only_send("recived".encode())  
            print(k.decode("utf-8")) 
            if k.decode("utf-8").find("Все отправлено") >= 0:
                break
            else:
                if k.decode("utf-8").find("не найдены") < 0:
                    k=n.wait() 
                    stra = stra[:-1]
                    # Decode base64 String Data
                    decodedData = base64.b64decode((stra))
                    tmp = k.decode("utf-8").split("\00")[0]
                    # Write Image from Base64 File
                    imgFile = open(tmp, 'wb')
                    imgFile.write(decodedData)
                    imgFile.close()   
                    stra=""            

main()