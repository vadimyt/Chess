#importing required image
import base64
with open("tmp.png", "rb") as imagefile:
    convert = base64.b64encode(imagefile.read())

print((convert.decode('utf-8')))

# Decode base64 String Data
decodedData = base64.b64decode((convert))
  
# Write Image from Base64 File
imgFile = open('image.png', 'wb')
imgFile.write(decodedData)
imgFile.close()