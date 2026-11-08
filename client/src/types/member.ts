export interface Member {
  id: string
  dateOfBirth: string
  imageUrl?: string
  displayName: string
  created: string
  lastActive: string
  gender: string
  description?: string
  city: string
  country: string
  photos: Photo[]
}

export interface Photo {
  id: number
  url: string
  publicId?: string
}

export interface EditableMember {
  displayName: string;
  description?: string;
  city: string;
  country: string;
}
