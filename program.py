from firebase import firebase
old = (0.0, 0.0, 0.0)
db = firebase.FirebaseApplication('https://unityrasp-66ac0-default-rtdb.asia-southeast1.firebasedatabase.app/', None)
while True:
    str_data: str = db.get('Data', None)
    str_data = str_data.replace('(', '').replace(')', '').split(',')
    x = float(str_data[0])
    y = float(str_data[1])
    z = float(str_data[2])
    new = (x, y, z)
    if old[0] != new[0]:
        print('X axis changed')
    if old[1] != new[1]:
        print('Y axis changed')
    if old[2] != new[2]:
        print('Z axis changed')
    if old != new:
        old = new
        print(f'Update: {new}')
        
